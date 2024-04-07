namespace FlowSynx.Cli.Commands.Storage.Move;

internal class MoveCommandOptions : ICommandOptions
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public string? MinSize { get; set; } = string.Empty;
    public string? MaxSize { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
}