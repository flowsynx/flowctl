namespace FlowCtl.Commands.Workflows.Triggers.Delete;

internal class DeleteWorkflowTriggerCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public required string TriggerId { get; set; }
    public string? Address { get; set; } = string.Empty;
}