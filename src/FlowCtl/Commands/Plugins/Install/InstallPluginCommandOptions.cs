namespace FlowCtl.Commands.Plugins.Install;

internal class InstallPluginCommandOptions : ICommandOptions
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Address { get; set; } = string.Empty;
}