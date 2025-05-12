using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Triggers.Details;

internal class WorkflowTriggerDetailsCommand
    : BaseCommand<WorkflowTriggerDetailsCommandOptions, WorkflowTriggerDetailsCommandOptionsHandler>
{
    public WorkflowTriggerDetailsCommand() : base("details", Resources.Commands_Workflows_DetailsDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_DetailsIdentityOption) { IsRequired = true };

        var executionIdOption = new Option<string>(new[] { "-t", "--trigger-id" },
            description: Resources.Commands_Workflows_DetailsIdentityOption) { IsRequired = true };

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