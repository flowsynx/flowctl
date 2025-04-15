using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Health;

internal class HealthCommandOptions : ICommandOptions
{
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}