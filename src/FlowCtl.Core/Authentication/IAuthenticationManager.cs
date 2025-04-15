namespace FlowCtl.Core.Authentication;

public interface IAuthenticationManager
{
    bool IsLoggedIn { get; }
    AuthenticationData LoginBasic(string username, string password);
    AuthenticationData LoginBearer(string token);
    void Logout();
}