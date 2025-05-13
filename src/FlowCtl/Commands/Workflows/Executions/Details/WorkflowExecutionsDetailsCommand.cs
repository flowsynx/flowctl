using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Executions.Details;

internal class WorkflowExecutionDetailsCommand 
    : BaseCommand<WorkflowExecutionDetailsCommandOptions, WorkflowExecutionDetailsCommandOptionsHandler>
{
    public WorkflowExecutionDetailsCommand() : base("details", Resources.Commands_Workflow_Execution_DetailsDescription)
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
    }
}