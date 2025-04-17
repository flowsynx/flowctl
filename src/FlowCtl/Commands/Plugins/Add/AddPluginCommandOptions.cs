namespace FlowCtl.Commands.Plugins.Add;

internal class AddPluginCommandOptions : ICommandOptions
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Address { get; set; } = string.Empty;
}