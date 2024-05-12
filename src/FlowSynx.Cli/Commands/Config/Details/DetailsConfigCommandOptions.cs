namespace FlowSynx.Cli.Commands.Config.Details;

internal class DetailsConfigCommandOptions : ICommandOptions
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}