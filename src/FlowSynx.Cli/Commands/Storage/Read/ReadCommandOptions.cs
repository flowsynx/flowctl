namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public bool? Hashing { get; set; } = false;
    public string SaveTo { get; set; } = string.Empty;
    public bool? Overwrite { get; set; } = false;
    public string? Address { get; set; } = string.Empty;
}