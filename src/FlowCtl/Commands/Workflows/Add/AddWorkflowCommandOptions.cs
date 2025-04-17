namespace FlowCtl.Commands.Workflows.Add;

internal class AddWorkflowCommandOptions : ICommandOptions
{
    public string Definition { get; set; } = string.Empty;
    public string? DefinitionFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}