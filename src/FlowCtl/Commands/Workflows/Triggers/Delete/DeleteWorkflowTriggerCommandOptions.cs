namespace FlowCtl.Commands.Workflows.Triggers.Details;

internal class DeleteWorkflowTriggerCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public required string TriggerId { get; set; }
    public string? Address { get; set; } = string.Empty;
}