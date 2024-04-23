namespace FlowSynx.Cli.Commands.Storage.Write;

internal class WriteCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Data { get; set; } = string.Empty;
    public string? FileToUpload { get; set; } = string.Empty;
    public bool Overwrite { get; set; } = false;
    public string? Url { get; set; } = string.Empty;
}