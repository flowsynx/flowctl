using System.CommandLine;
using FlowCtl.Commands.Workflows.Add;
using FlowCtl.Commands.Workflows.Delete;
using FlowCtl.Commands.Workflows.Details;
using FlowCtl.Commands.Workflows.Execute;
using FlowCtl.Commands.Workflows.Update;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Workflows;

internal class WorkflowsCommand : BaseCommand<WorkflowsCommandOptions, WorkflowsCommandOptionsHandler>
{
    public WorkflowsCommand() : base("workflows", Resources.ConnectorsCommandDescription)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);

        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new AddWorkflowCommand());
        AddCommand(new DeleteWorkflowCommand());
        AddCommand(new ExecuteWorkflowCommand());
        AddCommand(new WorkflowDetailsCommand());
        AddCommand(new UpdateWorkflowCommand());
    }
}