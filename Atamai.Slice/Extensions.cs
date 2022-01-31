using Atamai.Slice.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Atamai.Slice;

public static class Extensions
{
    // ReSharper disable once CollectionNeverUpdated.Global
    private static readonly List<AtamaiSlice> Routers = new();

    public static void UseSlice(this WebApplication endpointRouteBuilder)
    {
        endpointRouteBuilder.UseAuthorizationMiddleware();

        foreach (var router in Routers)
        {
            router.Register(endpointRouteBuilder);
        }

        Routers.Clear();
    }

    public static void AddSlice(this WebApplicationBuilder builder)
    {
        var serviceCollection = builder.Services;
        foreach (var router in Routers)
        {
            router.ConfigureServices(serviceCollection);
        }
    }

    // Used by Atamai.Slice.Generator
    // ReSharper disable once UnusedMember.Global
    public static void Add<T>() where T : AtamaiSlice, new()
    {
        Routers.Add(new T());
    }
}