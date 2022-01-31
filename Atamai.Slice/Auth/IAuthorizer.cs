namespace Atamai.Slice.Auth;

public interface IAuthorizer
{
    bool Authorize(string apiKey);
}