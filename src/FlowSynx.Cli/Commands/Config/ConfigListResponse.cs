namespace FlowSynx.Cli.Commands.Config;

public class ConfigListResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
}