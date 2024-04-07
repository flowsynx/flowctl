namespace FlowSynx.Cli.Commands.Storage.About;

internal class AboutRequest
{
    public string? Path { get; set; } = string.Empty;
    public bool? Full { get; set; } = false;
}