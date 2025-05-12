using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Triggers.Details;

internal class DeleteWorkflowTriggerCommand 
    : BaseCommand<DeleteWorkflowTriggerCommandOptions, DeleteWorkflowTriggerCommandOptionsHandler>
{
    public DeleteWorkflowTriggerCommand() : base("delete", Resources.Commands_Workflows_DeleteDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_DeleteIdentityOption) { IsRequired = true };

        var triggerIdOption = new Option<string>(new[] { "-t", "--trigger-id" },
            description: Resources.Commands_Workflows_DeleteIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(triggerIdOption);
        AddOption(addressOption);
    }
}