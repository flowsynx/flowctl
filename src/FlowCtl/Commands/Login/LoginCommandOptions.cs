namespace FlowCtl.Commands.Login;

internal class LoginCommandOptions : ICommandOptions
{
    public bool? Basic { get; set; } = false;
    public bool? Bearer { get; set; } = false;
    public bool? OAuth { get; set; } = false;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public string? Authority { get; set; }
    public string? ClientId { get; set; }
    public string? Scope { get; set; }
}