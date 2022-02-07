namespace Atamai.Slice.Sample;

public static class Extensions
{
    public static string AuthorizationToken(this HttpContext httpContext)
    {
        return httpContext.Request.Headers.Authorization[0];
    }
}