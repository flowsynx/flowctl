using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Triggers.Add;

internal class AddWorkflowtriggerCommand : BaseCommand<AddWorkflowTriggerCommandOptions, AddWorkflowTriggerCommandOptionsHandler>
{
    public AddWorkflowtriggerCommand() : base("add", Resources.Commands_Workflows_AddDescription)
    {
        var workflowIdOption = new Option<string?>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_Add_DefinitionData);

        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.Commands_Workflows_Add_DefinitionData);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.Commands_Workflows_Add_DefinitionDataFile);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
    }
}