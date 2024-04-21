using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", "Delete configuration section")
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" }, "The configuration section name") { IsRequired = true };

        AddOption(nameOption);
    }
}