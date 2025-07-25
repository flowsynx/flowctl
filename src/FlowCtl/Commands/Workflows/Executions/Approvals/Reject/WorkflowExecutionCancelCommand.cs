using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Approvals.Reject;

internal class WorkflowExecutionRejectPendingTaskCommand
    : BaseCommand<WorkflowExecutionRejectPendingTaskCommandOptions, WorkflowExecutionRejectPendingTaskCommandOptionsHandler>
{
    public WorkflowExecutionRejectPendingTaskCommand() : base("reject", Resources.Commands_Workflow_Execution_RejectPendingTaskDescription)
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