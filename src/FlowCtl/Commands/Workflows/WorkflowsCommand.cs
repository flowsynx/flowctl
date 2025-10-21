using System.CommandLine;
using FlowCtl.Commands.Workflows.Add;
using FlowCtl.Commands.Workflows.Delete;
using FlowCtl.Commands.Workflows.Details;
using FlowCtl.Commands.Workflows.Executions;
using FlowCtl.Commands.Workflows.Triggers;
using FlowCtl.Commands.Workflows.Update;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows;

internal class WorkflowsCommand : BaseCommand<WorkflowsCommandOptions, WorkflowsCommandOptionsHandler>
{
    public WorkflowsCommand() : base("workflows", Resources.Commands_Workflows_Description)
    {
        var pageOption = new Option<int?>(new[] { "-p", "--page" },
            description: Resources.Commands_FlowSynxPage);

        var pageSizeOption = new Option<int?>(new[] { "-s", "--page-size" },
            description: Resources.Commands_FlowSynxPageSize);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(pageOption);
        AddOption(pageSizeOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new AddWorkflowCommand());
        AddCommand(new DeleteWorkflowCommand());
        AddCommand(new WorkflowDetailsCommand());
        AddCommand(new UpdateWorkflowCommand());
        AddCommand(new WorkflowExecutionsCommand());
        AddCommand(new WorkflowTriggersCommand());
    }
}