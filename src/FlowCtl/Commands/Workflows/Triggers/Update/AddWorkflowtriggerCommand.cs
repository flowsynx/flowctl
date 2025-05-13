using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Triggers.Update;

internal class UpdateWorkflowtriggerCommand : BaseCommand<UpdateWorkflowTriggerCommandOptions, UpdateWorkflowTriggerCommandOptionsHandler>
{
    public UpdateWorkflowtriggerCommand() : base("update", Resources.Commands_Workflows_AddDescription)
    {
        var workflowIdOption = new Option<string?>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption);

        var triggerIdOption = new Option<string?>(new[] { "-t", "--trigger-id" },
            description: Resources.Commands_Workflows_Triggers_IdentityOption);

        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.Commands_Workflows_Triggers_Update_DefinitionData);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.Commands_Workflows_Triggers_Update_DefinitionDataFile);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(triggerIdOption);
        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
    }
}