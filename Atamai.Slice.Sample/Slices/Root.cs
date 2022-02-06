using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices;

public class Root : IApiSlice
{
    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/", () => "Hello")
        .WithDescription("Be nice")
        .AllowAnonymous();
}