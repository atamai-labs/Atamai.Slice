using Atamai.Slice.Swagger;
using Atamai.Slice.Validation;

namespace Atamai.Slice.Sample.Slices.Session;

public class Create : IApiSlice
{
    public record CreateSession(string Username, string Password) : IValidatable
    {
        void IValidatable.Validate(ValidationContext context)
        {
            context.NotNull(Username)
                ?.Custom(Username, static username => username.Length > 0, "Must be longer than 0 chars");

            context.Custom(Password, static password => !string.IsNullOrWhiteSpace(password));
        }
    }

    public static void Register(IEndpointRouteBuilder builder) => builder
        .MapPost("/session", (CreateSession request, DataBase dataBase) =>
        {
            if (request.Validate() is { } problem)
                return problem;

            if (dataBase.Users.TryGetValue(request.Username, out var hashedPassword) &&
                PasswordHasher.Compare(hashedPassword, request.Password))
            {
                var apiKey = Guid.NewGuid().ToString("N");
                dataBase.ApiKeyUser[apiKey] = request.Username;

                return Results.Ok(apiKey);
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