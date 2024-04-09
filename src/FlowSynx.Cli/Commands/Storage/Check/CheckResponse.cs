namespace FlowSynx.Cli.Commands.Storage.Check;

internal class CheckResponse
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string State { get; set; } = string.Empty;
}