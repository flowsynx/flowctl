using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Update;

internal class UpdateWorkflowCommand : BaseCommand<UpdateWorkflowCommandOptions, UpdateWorkflowCommandOptionsHandler>
{
    public UpdateWorkflowCommand() : base("update", Resources.Commands_Workflows_UpdateDescription)
    {
        var workflowIdOption = new Option<string?>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_UpdateIdentityOption) { IsRequired = true };

        var definitionOption = new Option<string?>(new[] { "-d", "--definition" },
            description: Resources.Commands_Workflows_Update_DefinitionData);

        var definitionFileOption = new Option<string?>(new[] { "-f", "--definition-file" },
            description: Resources.Commands_Workflows_Update_DefinitionDataFile);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(definitionOption);
        AddOption(definitionFileOption);
        AddOption(addressOption);
    }
}