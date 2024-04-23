namespace FlowSynx.Cli.Commands.Health;

internal class HealthCommandOptions : ICommandOptions
{
    public string? Url { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}