using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Atamai.Slice.Authorization;

internal static class AuthorizationMiddleware
{
    public const string Scheme = "api-key";

    public static void UseAuthorizationMiddleware(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
                return next(context);

            if(context.Request.Headers.Authorization.Count > 0)
                return TryAuthorize(context, next);

            context.Response.Headers.WWWAuthenticate = Scheme;
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        });
    }

    private static async Task TryAuthorize(HttpContext httpContext, RequestDelegate next)
    {
        var authorizer = httpContext.RequestServices.GetRequiredService<IAuthorizer>();

        if (await authorizer.Authorize(httpContext))
        {
            await next(httpContext);
        }
        else
        {
            httpContext.Response.Headers.WWWAuthenticate = Scheme;
            httpContext.Response.StatusCode = 401;
        }
    }
}