# Atamai.Slice

Proof-of-concept solution for slicing minimal-api and using source generator to resolve the slices 
without runtime reflection or manual registrations.

Atamai.Slice also contains minimal auth that requires opt-out of just to play with the auth of a sliced api.

We use [Static abstract members in interfaces](https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/static-abstract-interface-methods) so .NET6 and `<EnablePreviewFeatures>True</EnablePreviewFeatures>` in csproj is required.

Example slice:
```c#
using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices.Session;

public class Create : IApiSlice
{
    public record CreateSession(string Username, string Password);

    public override void Register(IEndpointRouteBuilder builder) => builder
        .MapPost("/session", (CreateSession request, DataBase dataBase) =>
        {
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
        .AllowAnonymous();
}
```

[Atamai.Slice.Generator](Atamai.Slice.Generator) will find all implementations of `IApiSlice` and generate something like the following:
```c#
public static class GeneratedApiSliceRegistrations 
{ 
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Init() => Atamai.Slice.Extensions.OnLoad += OnLoad;

    private static void OnLoad(IEndpointRouteBuilder builder)
    {
        Atamai.Slice.Sample.Slices.Root.Register(builder);
        Atamai.Slice.Sample.Slices.Session.Create.Register(builder);
        Atamai.Slice.Sample.Slices.Session.Delete.Register(builder);
        Atamai.Slice.Sample.Slices.Session.Get.Register(builder);
        Atamai.Slice.Extensions.OnLoad -= OnLoad;
    }
}
```

`app.UseSlice();` will trigger the `OnLoad` event and register all slices 

Take a look at [Atamai.Slice.Sample](Atamai.Slice.Sample) to see it in action.

## Notes / Thoughts
- What if we have slices and a generated registration in an assembly that isn't the main startup assembly?
  - Adding something like a "dummy" method `App.AddSlice().AddAssemblyFromType<Type>` that just touches the type should work since it will invoke the `ModuleInitializer` on the `GeneratedApiSliceRegistrations` from that assembly.
