namespace FlowSynx.Cli.Commands.Plugins.Details;

internal class PluginDetailsCommandOptions : ICommandOptions
{
    public Guid Id { get; set; } = Guid.Empty;
    public string? Url { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}