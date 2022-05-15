namespace Atamai.Slice.Sample.Slices;

public class Root : IApiSlice
{
    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/", () => "Hello")
        .Produces<string>(StatusCodes.Status200OK)
        .AllowAnonymous();
}