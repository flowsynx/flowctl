using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Cancel;

internal class WorkflowExecutionCancelCommand
    : BaseCommand<WorkflowExecutionCancelCommandOptions, WorkflowExecutionCancelCommandOptionsHandler>
{
    public WorkflowExecutionCancelCommand() : base("cancel", Resources.Commands_Workflows_DetailsDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_DetailsIdentityOption) { IsRequired = true };

        var executionIdOption = new Option<string>(new[] { "-e", "--execution-id" },
            description: Resources.Commands_Workflows_DetailsIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(executionIdOption);
        AddOption(addressOption);
    }
}