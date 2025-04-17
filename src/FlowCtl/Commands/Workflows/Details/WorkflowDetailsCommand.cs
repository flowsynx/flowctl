using FlowCtl.Core.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Details;

internal class WorkflowDetailsCommand : BaseCommand<WorkflowDetailsCommandOptions, WorkflowDetailsCommandOptionsHandler>
{
    public WorkflowDetailsCommand() : base("details", Resources.ConnectorDetailsCommandDescription)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);

        AddOption(identityOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}