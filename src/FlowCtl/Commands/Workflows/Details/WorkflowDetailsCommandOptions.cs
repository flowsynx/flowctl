using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows.Details;

internal class WorkflowDetailsCommandOptions : ICommandOptions
{
    public required string Id { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}