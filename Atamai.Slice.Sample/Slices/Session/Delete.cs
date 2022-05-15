namespace Atamai.Slice.Sample.Slices.Session;

public class Delete : IApiSlice
{
    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapDelete("/session", (string token, Database dataBase) =>
        {
            if (dataBase.TokenUser.TryRemove(token, out _))
                return Results.Ok();

            return Results.Unauthorized();
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
}