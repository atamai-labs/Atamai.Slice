using Atamai.Slice.Authentication;

namespace Atamai.Slice.Sample;

public class Authenticator : IAuthenticator
{
    private readonly Database _database;

    public Authenticator(Database database)
    {
        _database = database;
    }

    public string Token { get; private set; } = string.Empty;

    Task<bool> IAuthenticator.Authenticate(HttpContext httpContext)
    {
        Token = httpContext.Request.Headers.Authorization;
        var validToken = _database.TokenUser.ContainsKey(Token);

        return Task.FromResult(validToken);
    }
}