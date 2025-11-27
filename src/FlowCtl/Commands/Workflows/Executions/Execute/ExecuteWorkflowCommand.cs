using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Execute;

internal class ExecuteWorkflowCommand : BaseCommand<ExecuteWorkflowCommandOptions, ExecuteWorkflowCommandOptionsHandler>
{
    public ExecuteWorkflowCommand() : base("execute", Resources.Commands_Workflows_ExecuteDescription)
    {
        var workflowIdOption = new Option<string?>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption);

        var definitionOption = new Option<string?>(new[] { "-d", "--definition" },
            description: Resources.Commands_Workflows_Add_DefinitionData);

        var definitionFileOption = new Option<string?>(new[] { "-f", "--definition-file" },
            description: Resources.Commands_Workflows_Add_DefinitionDataFile);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(definitionOption);
        AddOption(definitionFileOption);
        AddOption(addressOption);
    }
}