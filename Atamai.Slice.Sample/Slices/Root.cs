using Atamai.Slice.Auth;
using Atamai.Slice.Swagger;

namespace Atamai.Slice.Sample.Slices;

/// <summary>
/// Root slice where we configure general services/routes
/// </summary>
public class Root : AtamaiSlice
{
    public override void Register(IEndpointRouteBuilder builder) => builder
        .MapGet("/", () => "Hello")
        .WithDescription("Be nice")
        .AllowAnonymous();

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<DataBase>();
        services.AddScoped<IAuthorizer, Authorizer>();
    }
}