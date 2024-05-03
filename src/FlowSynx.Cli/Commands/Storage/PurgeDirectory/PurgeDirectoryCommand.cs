using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.PurgeDirectory;

internal class PurgeDirectoryCommand : BaseCommand<PurgeDirectoryCommandOptions, PurgeDirectoryCommandOptionsHandler>
{
    public PurgeDirectoryCommand() : base("purge", "Delete the directory and its entities and contents on specific storage path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The storage's path that are the directory to be purged") { IsRequired = true };

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(urlOption);
    }
}