using Atamai.Slice.Auth;
using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Delete : IApiSlice
{
    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapDelete("/session", (HttpContext httpContext, DataBase dataBase) =>
        {
            var authorizationToken = httpContext.AuthorizationToken();
            if (dataBase.ApiKeyUser.TryRemove(authorizationToken, out _))
                return Results.Ok();

            return Results.Unauthorized();
        })
        .WithDescription("Delete session",
            @"A verbose explanation of the operation behavior.\
              [CommonMark](https://spec.commonmark.org/) syntax MAY be used for rich text representation.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
}