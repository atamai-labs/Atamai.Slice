using Microsoft.AspNetCore.Http;

namespace Atamai.Slice.Auth;

public interface IAuthorizer
{
    Task<bool> Authorize(HttpContext httpContext);
}