# Atamai.Slice

Proof-of-concept solution for slicing minimal-api and using source generator to resolve the slices 
without runtime reflection or manual registrations.

Atamai.Slice also contains dummy validation and minimal auth just to play with the structure of a sliced api.

Example slice:
```c#
using Atamai.Slice.Swagger;
using Atamai.Slice.Validation;

namespace Atamai.Slice.Sample.Slices.Session;

public class Create : IApiSlice
{
    public record CreateSession(string Username, string Password) : IValidatable
    {
        void IValidatable.Validate(in ValidationContext context)
        {
            context.NotNull(Username)
                ?.Custom(Username, static username => username.Length > 0, "Must be longer than 0 chars");

            context.Custom(Password, static password => !string.IsNullOrWhiteSpace(password));
        }
    }

    public override void Register(IEndpointRouteBuilder builder) => builder
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
        .WithDescription("Create Session")
        .Produces<string>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesValidationProblem()
        .AllowAnonymous();
}
```

[Atamai.Slice.Generator](Atamai.Slice.Generator) will find all implementations of `IApiSlice` and generate something like the following:
```c#
public static class GeneratedApiSliceRegistrations 
{ 
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Init() 
    {
        Atamai.Slice.Extensions.Add<Atamai.Slice.Sample.Slices.Root>();
        Atamai.Slice.Extensions.Add<Atamai.Slice.Sample.Slices.Session.Create>();
        Atamai.Slice.Extensions.Add<Atamai.Slice.Sample.Slices.Session.Delete>();
    }
}
```

This will register all slices on application startup so it's available for the `builder.AddSlice();` and `app.UseSlice();` 

Take a look at [Atamai.Slice.Sample](Atamai.Slice.Sample) to see it in action.

## Notes / Thoughts
- What if we have slices and a generated registration in an assembly that isn't the main startup assembly?
  - Adding something like a "dummy" method `App.AddSlice().AddAssemblyFromType<Type>` that just touches the type should work since it will invoke the `ModuleInitializer` on the `GeneratedApiSliceRegistrations` from that assembly.

## Todo
- [ ] Validate if this idea is something that should exist
  - [ ] Add tests
  - [ ] Make it pretty
  - [x] Package nuget
  - [ ] Compare startup performance with MVC and Carter