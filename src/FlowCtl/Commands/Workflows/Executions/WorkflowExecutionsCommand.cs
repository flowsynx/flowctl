using FlowCtl.Commands.Workflows.Executions.Approvals;
using FlowCtl.Commands.Workflows.Executions.Cancel;
using FlowCtl.Commands.Workflows.Executions.Details;
using FlowCtl.Commands.Workflows.Executions.Execute;
using FlowCtl.Commands.Workflows.Executions.Logs;
using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions;

internal class WorkflowExecutionsCommand : BaseCommand<WorkflowExecutionsCommandOptions, WorkflowExecutionsCommandOptionsHandler>
{
    public WorkflowExecutionsCommand() : base("executions", Resources.Commands_Workflows_ExecutionsListDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_IdentityOption) { IsRequired = true };

        var pageOption = new Option<int?>(new[] { "-p", "--page" },
            description: Resources.Commands_FlowSynxPage);

        var pageSizeOption = new Option<int?>(new[] { "-s", "--page-size" },
            description: Resources.Commands_FlowSynxPageSize);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(workflowIdOption);
        AddOption(pageOption);
        AddOption(pageSizeOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);

        AddCommand(new WorkflowExecutionPendingApprovalsCommand());
        AddCommand(new WorkflowExecutionCancelCommand());
        AddCommand(new WorkflowExecutionDetailsCommand());
        AddCommand(new ExecuteWorkflowCommand());
        AddCommand(new WorkflowExecutionLogsCommand());
    }
}