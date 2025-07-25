using FlowCtl.Commands.Workflows.Executions.Approvals.Approve;
using FlowCtl.Commands.Workflows.Executions.Approvals.Reject;
using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Approvals;

internal class WorkflowExecutionPendingApprovalsCommand 
    : BaseCommand<WorkflowExecutionPendingApprovalsCommandOptions, WorkflowExecutionPendingApprovalsCommandOptionsHandler>
{
    public WorkflowExecutionPendingApprovalsCommand() : base("approvals", Resources.Commands_Workflow_Execution_ApprovalsDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption) { IsRequired = true };

        var executionIdOption = new Option<string>(new[] { "-e", "--execution-id" },
            description: Resources.Commands_Workflows_Execution_IdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(workflowIdOption);
        AddOption(executionIdOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);

        AddCommand(new WorkflowExecutionApprovePendingTaskCommand());
        AddCommand(new WorkflowExecutionRejectPendingTaskCommand());
    }
}