using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", "Delete configuration section")
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" },
            description: "The configuration section name") { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        AddOption(nameOption);
        AddOption(addressOption);
    }
}