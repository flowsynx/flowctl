using System.CommandLine;

namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", Resources.Commands_DeleteConfig_Description)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.Commands_DeleteConfig_IdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(identityOption);
        AddOption(addressOption);
    }
}