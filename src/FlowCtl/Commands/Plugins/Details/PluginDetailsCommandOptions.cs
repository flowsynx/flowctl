using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Plugins.Details;

internal class PluginDetailsCommandOptions : ICommandOptions
{
    public required string Id { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}