using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices;

public class Root : IApiSlice
{
    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/", () => "Hello")
        .WithDescription("Be nice",
            @"A verbose explanation of the operation behavior.\
              [CommonMark](https://spec.commonmark.org/) syntax MAY be used for rich text representation.")
        .Produces<string>(StatusCodes.Status200OK)
        .AllowAnonymous();
}