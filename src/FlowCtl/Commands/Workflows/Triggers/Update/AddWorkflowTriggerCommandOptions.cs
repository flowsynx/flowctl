namespace FlowCtl.Commands.Workflows.Triggers.Update;

internal class UpdateWorkflowTriggerCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public required string TriggerId { get; set; }
    public string Data { get; set; } = string.Empty;
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}