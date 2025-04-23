using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Delete;

internal class DeleteWorkflowCommand : BaseCommand<DeleteWorkflowCommandOptions, DeleteWorkflowCommandOptionsHandler>
{
    public DeleteWorkflowCommand() : base("delete", Resources.Commands_Workflows_DeleteDescription)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.Commands_Workflows_DeleteIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(identityOption);
        AddOption(addressOption);
    }
}