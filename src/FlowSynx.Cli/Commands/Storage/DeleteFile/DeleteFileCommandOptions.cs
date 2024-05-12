namespace FlowSynx.Cli.Commands.Storage.DeleteFile;

internal class DeleteFileCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}