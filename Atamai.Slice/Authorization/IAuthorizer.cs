using Microsoft.AspNetCore.Http;

namespace Atamai.Slice.Authorization;

public interface IAuthorizer
{
    Task<bool> Authorize(HttpContext httpContext);
}