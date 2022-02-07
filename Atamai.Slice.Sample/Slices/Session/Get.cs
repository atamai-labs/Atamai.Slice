using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Get : IApiSlice
{
    public record Session(string Username, string Token);

    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/session", (Authenticator authenticator, DataBase dataBase) =>
        {
            var token = authenticator.Token;

            if (dataBase.TokenUser.TryGetValue(token, out var user))
                return Results.Ok(new Session(user, token));

            return Results.NotFound();
        })
        .WithDescription("Secure secret information",
            @"A verbose explanation of the operation behavior.\
              [CommonMark](https://spec.commonmark.org/) syntax MAY be used for rich text representation.")
        .Produces<Session>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
}