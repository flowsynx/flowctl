using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.PurgeDirectory;

internal class PurgeDirectoryCommand : BaseCommand<PurgeDirectoryCommandOptions, PurgeDirectoryCommandOptionsHandler>
{
    public PurgeDirectoryCommand() : base("purge", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };

        AddOption(pathOption);
    }
}