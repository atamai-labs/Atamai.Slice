using Atamai.Slice.Auth;

namespace Atamai.Slice.Sample;

public class Authorizer : IAuthorizer
{
    private readonly DataBase _dataBase;

    public Authorizer(DataBase dataBase)
    {
        _dataBase = dataBase;
    }

    public Task<bool> Authorize(HttpContext httpContext)
    {
        var apiKey = httpContext.Request.Headers.Authorization;
        return Task.FromResult(_dataBase.ApiKeyUser.ContainsKey(apiKey));
    }
}