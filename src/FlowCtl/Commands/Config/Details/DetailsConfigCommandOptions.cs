using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Config.Details;

internal class DetailsConfigCommandOptions : ICommandOptions
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}