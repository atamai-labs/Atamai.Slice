using Atamai.Slice.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atamai.Slice.Swagger;

public static class SwaggerSecurity
{
    public static void Apply(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("api-key", new OpenApiSecurityScheme
        {
            Description = nameof(AuthorizationMiddleware),
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "basic"
        });
    }

    public static void Apply(OpenApiOperation operation)
    {
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "api-key"
                        },
                        Scheme = "basic",
                        Name = "Authorization",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            }
        };
    }
}