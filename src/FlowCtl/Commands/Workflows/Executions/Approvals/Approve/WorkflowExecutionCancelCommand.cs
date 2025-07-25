using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Approvals.Approve;

internal class WorkflowExecutionApprovePendingTaskCommand
    : BaseCommand<WorkflowExecutionApprovePendingTaskCommandOptions, WorkflowExecutionApprovePendingTaskCommandOptionsHandler>
{
    public WorkflowExecutionApprovePendingTaskCommand() : base("approve", Resources.Commands_Workflow_Execution_ApprovePendingTaskDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption) { IsRequired = true };

        var executionIdOption = new Option<string>(new[] { "-e", "--execution-id" },
            description: Resources.Commands_Workflows_Execution_IdentityOption) { IsRequired = true };

        var approvalIdOption = new Option<string>(new[] { "-p", "--approval-id" },
            description: Resources.Commands_Workflows_Execution_ApprovalIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(workflowIdOption);
        AddOption(executionIdOption);
        AddOption(approvalIdOption);
        AddOption(addressOption);
    }
}