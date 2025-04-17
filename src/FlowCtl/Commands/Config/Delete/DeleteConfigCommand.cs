using System.CommandLine;

namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", Resources.DeleteConfigCommandDescription)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.CommandFieldOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(identityOption);
        AddOption(addressOption);
    }
}