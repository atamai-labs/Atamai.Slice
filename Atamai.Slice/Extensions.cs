using Atamai.Slice.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Atamai.Slice;

public static class Extensions
{
    private static readonly List<Action<IEndpointRouteBuilder>> Slices = new();

    public static void UseSlice(this WebApplication endpointRouteBuilder)
    {
        endpointRouteBuilder.UseAuthorizationMiddleware();

        foreach (var slice in Slices)
        {
            slice(endpointRouteBuilder);
        }

        Slices.Clear();
    }

    public static void Add<T>() where T : IApiSlice, new()
    {
        Slices.Add(static builder => T.Register(builder));
    }
}