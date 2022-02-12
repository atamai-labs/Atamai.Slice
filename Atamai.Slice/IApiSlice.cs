using Microsoft.AspNetCore.Routing;

namespace Atamai.Slice;

public interface IApiSlice
{
    // ReSharper disable once UnusedMemberInSuper.Global
    // Used by generator
    public static abstract void Register(IEndpointRouteBuilder builder);
}