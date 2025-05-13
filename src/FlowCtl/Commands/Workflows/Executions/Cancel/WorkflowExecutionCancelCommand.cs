using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Cancel;

internal class WorkflowExecutionCancelCommand
    : BaseCommand<WorkflowExecutionCancelCommandOptions, WorkflowExecutionCancelCommandOptionsHandler>
{
    public WorkflowExecutionCancelCommand() : base("cancel", Resources.Commands_Workflow_Execution_CancelDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption) { IsRequired = true };

        var executionIdOption = new Option<string>(new[] { "-e", "--execution-id" },
            description: Resources.Commands_Workflows_Execution_IdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(executionIdOption);
        AddOption(addressOption);
    }
}