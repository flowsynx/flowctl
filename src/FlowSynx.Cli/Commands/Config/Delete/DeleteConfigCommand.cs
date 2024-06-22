using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", Resources.DeleteConfigCommandDescription)
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" },
            description: Resources.DeleteConfigCommandNameOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(nameOption);
        AddOption(addressOption);
    }
}