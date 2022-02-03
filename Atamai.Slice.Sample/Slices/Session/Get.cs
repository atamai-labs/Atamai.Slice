using Atamai.Slice.Auth;
using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Get : AtamaiSlice
{
    public record Session(string Username, string Token);

    public override void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/session", (HttpContext httpContext, DataBase dataBase) =>
        {
            var authorizationToken = httpContext.AuthorizationToken();

            if (dataBase.ApiKeyUser.TryGetValue(authorizationToken, out var user))
                return Results.Ok(new Session(user, authorizationToken));

            return Results.NotFound();
        })
        .WithDescription("Secure secret information")
        .Produces<Session>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}