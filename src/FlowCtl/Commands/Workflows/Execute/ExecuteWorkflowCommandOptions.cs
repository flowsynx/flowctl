namespace FlowCtl.Commands.Workflows.Execute;

internal class ExecuteWorkflowCommandOptions : ICommandOptions
{
    public required string Id { get; set; }
    public string? Address { get; set; } = string.Empty;
}