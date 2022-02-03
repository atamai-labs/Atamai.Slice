using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Atamai.Slice.Auth;

public static class AuthorizationMiddleware
{
    public const string Scheme = "api-key";

    public static void UseAuthorizationMiddleware(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
                return next(context);

            var authorizationHeader = context.Request.Headers.Authorization;
            if(authorizationHeader.Count > 0)
                return TryAuthorize(authorizationHeader, context, next);

            context.Response.Headers.WWWAuthenticate = Scheme;
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        });
    }

    private static async Task TryAuthorize(StringValues authorizationHeader, HttpContext httpContext, RequestDelegate next)
    {
        var authorizer = httpContext.RequestServices.GetRequiredService<IAuthorizer>();

        if (await authorizer.Authorize(authorizationHeader[0]))
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