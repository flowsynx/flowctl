namespace FlowSynx.Cli.Commands.Storage.Copy;

internal class CopyCommandOptions : ICommandOptions
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public bool? ClearDestinationPath { get; set; } = false;
    public bool? OverWriteData { get; set; } = false;
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public string? MinSize { get; set; } = string.Empty;
    public string? MaxSize { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
    public string? Address { get; set; } = string.Empty;
}