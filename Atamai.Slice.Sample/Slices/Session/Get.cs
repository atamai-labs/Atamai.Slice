namespace Atamai.Slice.Sample.Slices.Session;

public class Get : IApiSlice
{
    public record Session(string Username, string Token);

    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/session", (string token, Database dataBase) =>
        {

            if (dataBase.TokenUser.TryGetValue(token, out var user))
                return Results.Ok(new Session(user, token));

            return Results.NotFound();
        })
        .Produces<Session>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}