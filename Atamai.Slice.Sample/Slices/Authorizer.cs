using Atamai.Slice.Auth;

namespace Atamai.Slice.Sample.Slices;

public class Authorizer : IAuthorizer
{
    private readonly DataBase _dataBase;

    public Authorizer(DataBase dataBase)
    {
        _dataBase = dataBase;
    }

    public Task<bool> Authorize(string apiKey)
    {
        return Task.FromResult(_dataBase.ApiKeyUser.ContainsKey(apiKey));
    }
}