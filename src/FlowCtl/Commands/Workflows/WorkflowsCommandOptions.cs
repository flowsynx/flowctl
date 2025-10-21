using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows;

internal class WorkflowsCommandOptions : ICommandOptions
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}