namespace FlowSynx.Cli.Commands.Storage.Delete;

internal class DeleteCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
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