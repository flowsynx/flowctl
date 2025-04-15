using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Plugins.Details;

internal class PluginDetailsCommandOptions : ICommandOptions
{
    public string Type { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}