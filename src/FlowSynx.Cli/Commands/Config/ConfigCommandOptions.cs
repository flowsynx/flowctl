namespace FlowSynx.Cli.Commands.Config;

internal class ConfigCommandOptions : ICommandOptions
{
    public string Type { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}