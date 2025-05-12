using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows.Triggers.Details;

internal class WorkflowTriggerDetailsCommandOptions : ICommandOptions
{
    public required string WorkflowId { get; set; }
    public required string TriggerId { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}