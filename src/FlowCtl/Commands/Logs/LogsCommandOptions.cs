using FlowCtl.Core.Logger;
using FlowSynx.Client.Requests;

namespace FlowCtl.Commands.Logs;

internal class LogsCommandOptions : ICommandOptions
{
    public string? Data { get; set; }
    public string? DataFile { get; set; }
    public string ExportTo { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}