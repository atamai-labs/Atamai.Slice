using Atamai.Slice.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Atamai.Slice;

public static class Extensions
{
    public static event Action<IEndpointRouteBuilder>? OnLoad;

    public static void UseSlice(this WebApplication builder)
    {
        builder.UseAuthenticationMiddleware();
        OnLoad?.Invoke(builder);
    }
}