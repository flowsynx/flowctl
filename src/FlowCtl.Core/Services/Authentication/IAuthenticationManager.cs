namespace FlowCtl.Core.Services.Authentication;

public interface IAuthenticationManager
{
    bool IsLoggedIn { get; }
    bool IsBasicAuthenticationUsed { get; }
    AuthenticationData LoginNone();
    AuthenticationData LoginBasic(string username, string password);
    AuthenticationData LoginBearer(string token);
    AuthenticationData? GetData();
    void Logout();
}