namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommandOptions : ICommandOptions
{
    public required string Id { get; set; }
    public string? Address { get; set; } = string.Empty;
}