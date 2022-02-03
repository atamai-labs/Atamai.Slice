namespace Atamai.Slice.Auth;

public interface IAuthorizer
{
    Task<bool> Authorize(string apiKey);
}