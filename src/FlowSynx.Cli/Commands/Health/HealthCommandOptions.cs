namespace FlowSynx.Cli.Commands.Health;

internal class HealthCommandOptions : ICommandOptions
{
    public Output Output { get; set; } = Output.Json;
}