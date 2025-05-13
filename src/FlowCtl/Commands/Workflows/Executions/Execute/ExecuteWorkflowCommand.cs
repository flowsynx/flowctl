using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Execute;

internal class ExecuteWorkflowCommand : BaseCommand<ExecuteWorkflowCommandOptions, ExecuteWorkflowCommandOptionsHandler>
{
    public ExecuteWorkflowCommand() : base("execute", Resources.Commands_Workflows_ExecuteDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption)
        { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(addressOption);
    }
}