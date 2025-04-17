using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Logs;

internal class LogsCommandOptions : ICommandOptions
{
    public string? Level { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Message { get; set; }
    public string? ExportTo { get; set; }
    public string? Address { get; set; }
    public OutputType Output { get; set; } = OutputType.Json;
}