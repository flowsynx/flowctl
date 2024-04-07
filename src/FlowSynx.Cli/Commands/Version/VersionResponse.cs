namespace FlowSynx.Cli.Commands.Version;

public class VersionResponse
{
    public string? Cli { get; set; }
    public required string FlowSynx { get; set; }
    public string? OSVersion { get; set; } = string.Empty;
    public string? OSArchitecture { get; set; } = string.Empty;
    public string? OSType { get; set; } = string.Empty;
}