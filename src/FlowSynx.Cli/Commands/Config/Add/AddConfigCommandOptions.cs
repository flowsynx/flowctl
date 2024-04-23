namespace FlowSynx.Cli.Commands.Config.Add;

internal class AddConfigCommandOptions : ICommandOptions
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Spec { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}