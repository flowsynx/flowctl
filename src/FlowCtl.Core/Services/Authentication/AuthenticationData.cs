namespace FlowCtl.Core.Services.Authentication;

public class AuthenticationData
{
    public AuthenticationType Type { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? AccessToken { get; set; }
    public DateTime? Expiry { get; set; }
}