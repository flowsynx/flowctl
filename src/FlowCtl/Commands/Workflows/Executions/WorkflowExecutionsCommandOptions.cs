using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows.Executions;

internal class WorkflowExecutionsCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}