using FlowSynx.Client.Requests;

namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommandOptions : ICommandOptions
{
    public string? Data { get; set; }
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}