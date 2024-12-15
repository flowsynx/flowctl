using FlowSynx.Client.Requests;

namespace FlowCtl.Commands.Connectors;

internal class ConnectorsCommandOptions : ICommandOptions
{
    public string? Data { get; set; }
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}