using Microsoft.AspNetCore.Routing;

namespace Atamai.Slice;

public interface IApiSlice
{
    public static abstract void Register(IEndpointRouteBuilder builder);
}