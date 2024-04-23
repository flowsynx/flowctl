namespace FlowSynx.Cli.Commands.Plugins;

internal class PluginsCommandOptions : ICommandOptions
{
    public string Type { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}