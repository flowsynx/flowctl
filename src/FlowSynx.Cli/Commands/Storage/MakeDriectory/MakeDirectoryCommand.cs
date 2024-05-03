using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.MakeDriectory;

internal class MakeDirectoryCommand : BaseCommand<MakeDirectoryCommandOptions, MakeDirectoryCommandOptionsHandler>
{
    public MakeDirectoryCommand() : base("mkdir", "Make the directory on specific storage path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The storage path to create the directory on") { IsRequired = true };

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(urlOption);
    }
}