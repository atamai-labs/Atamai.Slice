using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atamai.Slice.Swagger;

public static class SwaggerExtensions
{
    public static void AddSlice(this SwaggerGenOptions options)
    {
        SwaggerSecurity.Apply(options);

        options.OperationFilter<CustomOperationFilter>();
        options.SupportNonNullableReferenceTypes();
    }

    public static TBuilder WithDescription<TBuilder>(
        this TBuilder builder,
        string summary,
        string? description = default)
        where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Add(endpointBuilder => endpointBuilder.Metadata.Add(new SwaggerRouteDescription(summary, description)));

        return builder;
    }
}