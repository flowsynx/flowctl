namespace FlowSynx.Cli.Commands.Storage.MakeDriectory;

internal class MakeDirectoryCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}