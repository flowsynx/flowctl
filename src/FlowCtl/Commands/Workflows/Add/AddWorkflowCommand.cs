using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Add;

internal class AddWorkflowCommand : BaseCommand<AddWorkflowCommandOptions, AddWorkflowCommandOptionsHandler>
{
    public AddWorkflowCommand() : base("add", Resources.Commands_Workflows_AddDescription)
    {
        var definitionOption = new Option<string?>(new[] { "-d", "--definition" },
            description: Resources.Commands_Workflows_Add_DefinitionData);

        var definitionFileOption = new Option<string?>(new[] { "-f", "--definition-file" },
            description: Resources.Commands_Workflows_Add_DefinitionDataFile);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(definitionOption);
        AddOption(definitionFileOption);
        AddOption(addressOption);
    }
}