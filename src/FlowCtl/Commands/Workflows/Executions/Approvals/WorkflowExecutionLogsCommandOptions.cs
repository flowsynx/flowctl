using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows.Executions.Approvals;

internal class WorkflowExecutionPendingApprovalsCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public required string ExecutionId { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}