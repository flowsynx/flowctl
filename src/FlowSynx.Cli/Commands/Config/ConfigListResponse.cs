namespace FlowSynx.Cli.Commands.Config;

internal class ConfigListResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
}