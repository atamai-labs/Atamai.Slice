using Atamai.Slice.Authentication;

namespace Atamai.Slice.Sample;

public class Authenticator : IAuthenticator
{
    private readonly DataBase _dataBase;

    public Authenticator(DataBase dataBase)
    {
        _dataBase = dataBase;
    }

    public string Token { get; private set; } = string.Empty;

    Task<bool> IAuthenticator.Authenticate(HttpContext httpContext)
    {
        Token = httpContext.Request.Headers.Authorization;
        var validToken = _dataBase.TokenUser.ContainsKey(Token);

        return Task.FromResult(validToken);
    }
}