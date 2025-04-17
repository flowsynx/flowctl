using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Add;

internal class AddWorkflowCommand : BaseCommand<AddWorkflowCommandOptions, AddWorkflowCommandOptionsHandler>
{
    public AddWorkflowCommand() : base("add", Resources.ConnectorDetailsCommandDescription)
    {
        var definitionOption = new Option<string?>(new[] { "-d", "--definition" },
            description: Resources.CommandFieldOption);

        var definitionFileOption = new Option<string?>(new[] { "-f", "--definition-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(definitionOption);
        AddOption(definitionFileOption);
        AddOption(addressOption);
    }
}