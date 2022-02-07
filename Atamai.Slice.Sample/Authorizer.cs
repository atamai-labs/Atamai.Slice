using Atamai.Slice.Authorization;

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
        var token = httpContext.Request.Headers.Authorization;
        return Task.FromResult(_dataBase.TokenUser.ContainsKey(token));
    }
}