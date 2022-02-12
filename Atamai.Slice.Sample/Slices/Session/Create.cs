using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Create : IApiSlice
{
    public record CreateSession(string Username, string Password);

    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapPost("/session", (CreateSession request, DataBase dataBase) =>
        {
            if (dataBase.Users.TryGetValue(request.Username, out var hashedPassword) &&
                PasswordHasher.Compare(hashedPassword, request.Password))
            {
                var token = Guid.NewGuid().ToString("N");
                dataBase.TokenUser[token] = request.Username;

                return Results.Ok(token);
            }

            return Results.Unauthorized();
        })
        .WithDescription("Create Session",
            @"A verbose explanation of the operation behavior.\
              [CommonMark](https://spec.commonmark.org/) syntax MAY be used for rich text representation.")
        .Produces<string>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesValidationProblem()
        .AllowAnonymous();
}