namespace FlowSynx.Cli.Commands.Config.Add;

internal class AddConfigRequest
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public Dictionary<string, string?>? Specifications { get; set; }
}