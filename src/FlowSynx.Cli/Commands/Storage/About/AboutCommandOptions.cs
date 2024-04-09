namespace FlowSynx.Cli.Commands.Storage.About;

internal class AboutCommandOptions : ICommandOptions
{
    public string? Path { get; set; } = string.Empty;
    public bool? Full { get; set; } = false;
    public Output Output { get; set; } = Output.Json;
}