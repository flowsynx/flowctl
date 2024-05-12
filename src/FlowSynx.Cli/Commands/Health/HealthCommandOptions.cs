namespace FlowSynx.Cli.Commands.Health;

internal class HealthCommandOptions : ICommandOptions
{
    public string? Address { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}