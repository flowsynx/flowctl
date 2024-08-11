using FlowSynx.Logging;

namespace FlowCtl.Commands.Logs;

internal class LogsCommandOptions : ICommandOptions
{
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public string? MaxResults { get; set; }
    public string? Sorting { get; set; } = string.Empty;
    public string? Level { get; set; }
    public string ExportTo { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}