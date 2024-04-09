using FlowSynx.IO.Compression;

namespace FlowSynx.Cli.Commands.Storage.Compress;

internal class CompressCommandOptions : ICommandOptions
{
    public required string Path { get; set; }
    public ItemKind? Kind { get; set; } = ItemKind.FileAndDirectory;
    public string? Include { get; set; }
    public string? Exclude { get; set; }
    public string? MinAge { get; set; }
    public string? MaxAge { get; set; }
    public string? MinSize { get; set; }
    public string? MaxSize { get; set; }
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
    public int? MaxResults { get; set; }
    public bool? Hashing { get; set; } = false;
    public FlowSynx.IO.Compression.CompressType? CompressType { get; set; } = FlowSynx.IO.Compression.CompressType.Zip;
    public string SaveTo { get; set; } = string.Empty;
    public bool? Overwrite { get; set; } = false;
}