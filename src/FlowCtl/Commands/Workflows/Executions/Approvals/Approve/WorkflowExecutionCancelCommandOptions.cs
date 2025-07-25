using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows.Executions.Approvals.Approve;

internal class WorkflowExecutionApprovePendingTaskCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public required string ExecutionId { get; set; }
    public required string ApprovalId { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}