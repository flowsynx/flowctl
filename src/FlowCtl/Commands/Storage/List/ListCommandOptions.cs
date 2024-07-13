namespace FlowCtl.Commands.Storage.List;

internal class ListCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Kind { get; set; } = nameof(ItemKind.FileAndDirectory);
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public string? MinSize { get; set; } = string.Empty;
    public string? MaxSize { get; set; } = string.Empty;
    public bool? Full { get; set; } = false;
    public string? Sorting { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
    public bool? Hashing { get; set; } = false;
    public int? MaxResults { get; set; }
    public bool IncludeMetadata { get; set; } = false;
    public string? Address { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}