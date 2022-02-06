using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atamai.Slice.Generator;

/// <summary>
/// IIncrementalGenerator knowledge: https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
/// </summary>
[Generator]
public class AddModuleInitializer : IIncrementalGenerator
{
    private const string InterfaceName = "IApiSlice";
    public record struct Slice(ClassDeclarationSyntax ClassDeclaration, string Identifier, string NameSpace);

#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor ClassModifierWarning = new("ATAMAI001", "Modifier",
        $"Only public, non-static, non-abstract implementations of {InterfaceName} is used by generator", "",
        DiagnosticSeverity.Warning, true);
#pragma warning restore RS2008

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var slices = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (s, _) => TransformToSliceForGeneration(s))
            .Where(static m => !string.IsNullOrWhiteSpace(m.NameSpace));

        // Combine the selected items with the `Compilation`
        IncrementalValueProvider<(Compilation compilation, ImmutableArray<Slice> items)> compilationAndSlices =
            context.CompilationProvider.Combine(slices.Collect());

        // Generate the source
        context.RegisterSourceOutput(compilationAndSlices,
            static (spc, source) => Generate(source.compilation, source.items, spc));
    }

    private static Slice TransformToSliceForGeneration(GeneratorSyntaxContext ctx)
    {
        var declarationSyntax = (ClassDeclarationSyntax)ctx.Node;
        var nameSpace = GetNamespace(declarationSyntax);
        return new Slice(declarationSyntax, declarationSyntax.Identifier.ToString(), nameSpace);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode s)
    {
        if (s is ClassDeclarationSyntax { BaseList.Types: var baseTypes })
        {
            for (var i = 0; i < baseTypes.Count; i++)
            {
                if (baseTypes[i].ToString() == InterfaceName)
                    return true;
            }
        }

        return false;
    }

    private static void Generate(Compilation compilation, ImmutableArray<Slice> items,
        SourceProductionContext context)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(@"public static class GeneratedApiSliceRegistrations 
{ 
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Init() 
    {");

        foreach (var (classDeclaration, identifier, nameSpace) in items)
        {
            if (IsValidForGeneration(context, classDeclaration))
            {
                stringBuilder.AppendLine($"        Atamai.Slice.Extensions.Add<{nameSpace}.{identifier}>();");
            }
        }

        stringBuilder.AppendLine(@"    }
}");

        context.AddSource("GeneratedAtamaiSliceRegistrations.g.cs", stringBuilder.ToString());
    }

    private static bool IsValidForGeneration(SourceProductionContext context, ClassDeclarationSyntax classDeclaration)
    {
        var modifiers = classDeclaration.Modifiers;
        for (var i = 0; i < modifiers.Count; i++)
        {
            var kind = modifiers[i].Kind();
            if (kind
                is SyntaxKind.AbstractKeyword
                or SyntaxKind.StaticKeyword
                or SyntaxKind.PrivateKeyword
                or SyntaxKind.InternalKeyword)
            {
                var diagnostics = Diagnostic.Create(ClassModifierWarning, classDeclaration.GetLocation());
                context.ReportDiagnostic(diagnostics);
                return false;
            }
        }

        return true;
    }

    static string GetNamespace(SyntaxNode? potentialNamespaceParent)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        var nameSpace = string.Empty;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
}