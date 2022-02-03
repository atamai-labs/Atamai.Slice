using Atamai.Slice.Auth;
using Microsoft.AspNetCore.Builder;

namespace Atamai.Slice;

public static class Extensions
{
    private static readonly List<AtamaiSlice> Slices = new();

    public static void UseSlice(this WebApplication endpointRouteBuilder)
    {
        endpointRouteBuilder.UseAuthorizationMiddleware();

        foreach (var slice in Slices)
        {
            slice.Register(endpointRouteBuilder);
        }

        Slices.Clear();
    }

    public static void AddSlice(this WebApplicationBuilder builder)
    {
        var serviceCollection = builder.Services;
        foreach (var slice in Slices)
        {
            slice.ConfigureServices(serviceCollection);
        }
    }

    public static void Add<T>() where T : AtamaiSlice, new()
    {
        Slices.Add(new T());
    }
}