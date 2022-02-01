using Atamai.Slice.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Atamai.Slice;

public static class Extensions
{
    // ReSharper disable once CollectionNeverUpdated.Global
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

    // Used by Atamai.Slice.Generator
    // ReSharper disable once UnusedMember.Global
    public static void Add<T>() where T : AtamaiSlice, new()
    {
        Slices.Add(new T());
    }
}