namespace FlowCtl.Commands.Workflows.Executions.Execute;

internal class ExecuteWorkflowCommandOptions : ICommandOptions
{
    public string? WorkflowId { get; set; }
    public string Definition { get; set; } = string.Empty;
    public string? DefinitionFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}