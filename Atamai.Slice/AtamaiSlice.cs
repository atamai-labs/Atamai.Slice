using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Atamai.Slice;

public abstract class AtamaiSlice
{
    public abstract void Register(IEndpointRouteBuilder builder);

    public virtual void ConfigureServices(IServiceCollection services)
    {
    }
}