using Microsoft.AspNetCore.Http;

namespace Atamai.Slice.Authentication;

public interface IAuthenticator
{
    Task<bool> Authenticate(HttpContext httpContext);
}