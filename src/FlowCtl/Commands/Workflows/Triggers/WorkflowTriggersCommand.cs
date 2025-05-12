using FlowCtl.Commands.Workflows.Triggers.Add;
using FlowCtl.Commands.Workflows.Triggers.Details;
using FlowCtl.Commands.Workflows.Triggers.Update;
using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Triggers;

internal class WorkflowTriggersCommand : BaseCommand<WorkflowTriggersCommandOptions, WorkflowTriggersCommandOptionsHandler>
{
    public WorkflowTriggersCommand() : base("triggers", Resources.Commands_Workflows_ExecutionsListDescription)
    {
        var workflowIdOption = new Option<string>(new[] { "-w", "--workflow-id" },
            description: Resources.Commands_Workflows_Executions_IdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(workflowIdOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);

        AddCommand(new AddWorkflowtriggerCommand());
        AddCommand(new WorkflowTriggerDetailsCommand());
        AddCommand(new DeleteWorkflowTriggerCommand());
        AddCommand(new UpdateWorkflowtriggerCommand());
    }
}