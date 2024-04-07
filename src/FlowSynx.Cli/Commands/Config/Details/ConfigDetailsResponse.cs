namespace FlowSynx.Cli.Commands.Config.Details;

public class ConfigDetailsResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public Dictionary<string, string?>? Specifications { get; set; }
}