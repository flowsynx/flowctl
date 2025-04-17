namespace FlowCtl.Commands.Plugins.Update;

internal class UpdatePluginCommandOptions : ICommandOptions
{
    public required string Type { get; set; }
    public required string OldVersion { get; set; }
    public required string NewVersion { get; set; }
    public string? Address { get; set; } = string.Empty;
}