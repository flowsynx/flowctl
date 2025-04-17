namespace FlowCtl.Commands.Workflows.Delete;

internal class DeleteWorkflowCommandOptions : ICommandOptions
{
    public required string Id { get; set; }
    public string? Address { get; set; } = string.Empty;
}