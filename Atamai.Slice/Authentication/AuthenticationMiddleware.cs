using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Atamai.Slice.Authentication;

internal static class AuthenticationMiddleware
{
    public const string Scheme = "token";

    public static void UseAuthenticationMiddleware(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
                return next(context);

            if(context.Request.Headers.Authorization.Count > 0)
                return Authenticate(context, next);

            context.Response.Headers.WWWAuthenticate = Scheme;
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        });
    }

    private static async Task Authenticate(HttpContext httpContext, RequestDelegate next)
    {
        var authenticator = httpContext.RequestServices.GetRequiredService<IAuthenticator>();

        if (await authenticator.Authenticate(httpContext))
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