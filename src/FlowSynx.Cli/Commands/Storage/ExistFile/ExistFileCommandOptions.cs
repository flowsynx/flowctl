namespace FlowSynx.Cli.Commands.Storage.ExistFile;

internal class ExistFileCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
}