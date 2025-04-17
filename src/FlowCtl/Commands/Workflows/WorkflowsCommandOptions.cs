using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Workflows;

internal class WorkflowsCommandOptions : ICommandOptions
{
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}