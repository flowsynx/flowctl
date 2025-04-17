using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Config;

internal class ConfigCommandOptions : ICommandOptions
{
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}