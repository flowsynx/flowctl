namespace FlowSynx.Cli.Commands.Storage.MakeDriectory;

internal class MakeDirectoryCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}