namespace FlowCtl.Core.Authentication;

public interface IAuthenticationManager
{
    bool IsLoggedIn { get; }
    bool IsBasicAuthenticationUsed { get; }
    AuthenticationData LoginBasic(string username, string password);
    AuthenticationData LoginBearer(string token);
    Task<AuthenticationData> LoginOAuthAsync(string authority, string clientId, string? scope);
    AuthenticationData? GetData();
    void Logout();
}