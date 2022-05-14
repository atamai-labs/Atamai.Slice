using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Delete : IApiSlice
{
    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapDelete("/session", (Authenticator authenticator, Database dataBase) =>
        {
            var token = authenticator.Token;
            if (dataBase.TokenUser.TryRemove(token, out _))
                return Results.Ok();

            return Results.Unauthorized();
        })
        .WithDescription("Delete session",
            @"A verbose explanation of the operation behavior.\
              [CommonMark](https://spec.commonmark.org/) syntax MAY be used for rich text representation.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
}