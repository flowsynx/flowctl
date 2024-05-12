namespace FlowSynx.Cli.Commands.Config.Delete;

internal class DeleteConfigCommandOptions : ICommandOptions
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}