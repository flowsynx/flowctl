namespace FlowCtl.Commands.Workflows.Update;

internal class UpdateWorkflowCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public string Definition { get; set; } = string.Empty;
    public string? DefinitionFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}