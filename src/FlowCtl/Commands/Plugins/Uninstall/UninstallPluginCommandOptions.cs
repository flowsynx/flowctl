namespace FlowCtl.Commands.Plugins.Uninstall;

internal class UninstallPluginCommandOptions : ICommandOptions
{
    public required string Type { get; set; }
    public string? Address { get; set; } = string.Empty;
}