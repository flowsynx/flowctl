namespace FlowCtl.Commands.Workflows.Executions.Execute;

internal class ExecuteWorkflowCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public string? Address { get; set; } = string.Empty;
}