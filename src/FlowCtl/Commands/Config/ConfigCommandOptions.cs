namespace FlowCtl.Commands.Config;

internal class ConfigCommandOptions : ICommandOptions
{
    public string[]? Fields { get; set; } = Array.Empty<string>();
    public string? Filter { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public string? Sort { get; set; } = string.Empty;
    public string? Limit { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}