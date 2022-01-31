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
    public record struct Slice(string Identifier, string? NameSpace);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // #if DEBUG
        // SpinWait.SpinUntil(() => System.Diagnostics.Debugger.IsAttached);
        // #endif

        var slices = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m.NameSpace is not null);

        // Combine the selected items with the `Compilation`
        IncrementalValueProvider<(Compilation compilation, ImmutableArray<Slice> items)> compilationAndSlices =
            context.CompilationProvider.Combine(slices.Collect());

        // Generate the source
        context.RegisterSourceOutput(compilationAndSlices,
            static (spc, source) => Execute(source.compilation, source.items, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<Slice> items,
        SourceProductionContext context)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(@"public static class GeneratedAtamaiSliceRegistrations 
{ 
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Init() 
    {");

        foreach (var item in items)
        {
            stringBuilder.AppendLine($"        Atamai.Slice.Extensions.Add<{item.NameSpace}.{item.Identifier}>();");
        }

        stringBuilder.AppendLine(@"    }
}");

        context.AddSource("GeneratedAtamaiSliceRegistrations.g.cs", stringBuilder.ToString());
    }

    private static Slice GetSemanticTargetForGeneration(GeneratorSyntaxContext ctx)
    {
        var declarationSyntax = (ClassDeclarationSyntax)ctx.Node;
        var nameSpace = GetNamespace(declarationSyntax);
        return new Slice(declarationSyntax.Identifier.ToString(), nameSpace);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax cds && IsRouter(cds))
        {
            return true;
        }

        return false;
    }

    public static bool IsRouter(ClassDeclarationSyntax source)
    {
        var modifiers = source.Modifiers;
        for (var i = 0; i < modifiers.Count; i++)
        {
            var kind = modifiers[i].Kind();
            if (kind
                is SyntaxKind.AbstractKeyword
                or SyntaxKind.StaticKeyword
                or SyntaxKind.PrivateKeyword)
            {
                return false;
            }
        }

        if (source.BaseList?.Types is { } baseTypes)
        {
            for (var i = 0; i < baseTypes.Count; i++)
            {
                var type = baseTypes[i];
                if (type.ToString() == "AtamaiSlice")
                    return true;
            }
        }

        return false;
    }

    static string GetNamespace(SyntaxNode? potentialNamespaceParent)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

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