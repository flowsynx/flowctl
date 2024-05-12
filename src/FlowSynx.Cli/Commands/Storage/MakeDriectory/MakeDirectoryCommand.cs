using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.MakeDriectory;

internal class MakeDirectoryCommand : BaseCommand<MakeDirectoryCommandOptions, MakeDirectoryCommandOptionsHandler>
{
    public MakeDirectoryCommand() : base("mkdir", "Make the directory on specific storage path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The storage path to create the directory on") { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(addressOption);
    }
}