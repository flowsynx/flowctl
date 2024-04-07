namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string SaveTo { get; set; } = string.Empty;
    public bool? Overwrite { get; set; } = false;
}