using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Invoke;

internal class InvokeCommandOptions : ICommandOptions
{
    public required string Method { get; set; }
    public Verb Verb { get; set; } = Verb.Post;
    public string? Data { get; set; }
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}