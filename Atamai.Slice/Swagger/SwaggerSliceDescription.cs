using Microsoft.OpenApi.Models;

namespace Atamai.Slice.Swagger;

public record SwaggerSliceDescription(string Summary, string? Description = default)
{
    public void Apply(OpenApiOperation operation)
    {
        operation.Summary = Summary;

        if(Description is {})
            operation.Description = Description;
    }
}