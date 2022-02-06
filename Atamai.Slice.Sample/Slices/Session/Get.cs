using Atamai.Slice.Authorization;
using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Get : IApiSlice
{
    public record Session(string Username, string Token);

    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/session", (HttpContext httpContext, DataBase dataBase) =>
        {
            var authorizationToken = httpContext.AuthorizationToken();

            if (dataBase.ApiKeyUser.TryGetValue(authorizationToken, out var user))
                return Results.Ok(new Session(user, authorizationToken));

            return Results.NotFound();
        })
        .WithDescription("Secure secret information",
            @"A verbose explanation of the operation behavior.\
              [CommonMark](https://spec.commonmark.org/) syntax MAY be used for rich text representation.")
        .Produces<Session>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}