using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows.Triggers;

internal class WorkflowTriggersCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}