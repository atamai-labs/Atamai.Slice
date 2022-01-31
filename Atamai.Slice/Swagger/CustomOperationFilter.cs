using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atamai.Slice.Swagger;

public class CustomOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowAnonymous = false;
        foreach (var metadata in context.ApiDescription.ActionDescriptor.EndpointMetadata)
        {
            switch (metadata)
            {
                case AllowAnonymousAttribute:
                    allowAnonymous = true;
                    break;
                case SwaggerRouteDescription swaggerRouteDescription:
                    swaggerRouteDescription.Apply(operation);
                    break;
            }
        }

        if(!allowAnonymous)
            SwaggerSecurity.Apply(operation);
    }
}