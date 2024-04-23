namespace FlowSynx.Cli.Commands.Storage.PurgeDirectory;

internal class PurgeDirectoryCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}