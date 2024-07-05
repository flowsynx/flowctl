namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommandOptions : ICommandOptions
{
    public Output Output { get; set; } = Output.Json;
}