using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Config;

internal class ConfigCommandOptions : ICommandOptions
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}