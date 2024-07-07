namespace FlowCtl.Commands.Storage.PurgeDirectory;

internal class PurgeDirectoryCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}