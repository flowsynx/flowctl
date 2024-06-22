using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.PurgeDirectory;

internal class PurgeDirectoryCommand : BaseCommand<PurgeDirectoryCommandOptions, PurgeDirectoryCommandOptionsHandler>
{
    public PurgeDirectoryCommand() : base("purge", Resources.PurgeDirectoryCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.PurgeDirectoryCommandPathOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(pathOption);
        AddOption(addressOption);
    }
}