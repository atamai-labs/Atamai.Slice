using Microsoft.AspNetCore.Http;

namespace Atamai.Slice.Authorization;

public static class Extensions
{
    public static string AuthorizationToken(this HttpContext httpContext)
    {
        return httpContext.Request.Headers.Authorization[0];
    }
}