using System.CommandLine;

namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", Resources.DeleteConfigCommandDescription)
    {
        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.CommandFieldOption);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
    }
}