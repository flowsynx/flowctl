namespace FlowCtl.Commands.Workflows.Triggers.Add;

internal class AddWorkflowTriggerCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public string Data { get; set; } = string.Empty;
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}