using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Config;

internal class ConfigCommandOptions : ICommandOptions
{
    public string? Data { get; set; }
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}