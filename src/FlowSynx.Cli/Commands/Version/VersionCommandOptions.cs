namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommandOptions : ICommandOptions
{
    public bool? Full { get; set; } = false;
    public string? Url { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}