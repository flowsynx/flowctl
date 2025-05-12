using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Delete;

internal class DeleteWorkflowCommand : BaseCommand<DeleteWorkflowCommandOptions, DeleteWorkflowCommandOptionsHandler>
{
    public DeleteWorkflowCommand() : base("delete", Resources.Commands_Workflows_DeleteDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_DeleteIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(addressOption);
    }
}