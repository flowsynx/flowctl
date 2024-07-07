namespace FlowCtl.Commands.Storage.ExistFile;

internal class ExistFileCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}