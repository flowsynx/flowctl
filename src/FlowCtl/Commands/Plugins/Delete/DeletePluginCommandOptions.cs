using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Plugins.Delete;

internal class DeletePluginCommandOptions : ICommandOptions
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Address { get; set; } = string.Empty;
}