using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Update;

internal class UpdateWorkflowCommand : BaseCommand<UpdateWorkflowCommandOptions, UpdateWorkflowCommandOptionsHandler>
{
    public UpdateWorkflowCommand() : base("update", Resources.ConnectorDetailsCommandDescription)
    {
        var identityOption = new Option<string?>(new[] { "-i", "--id" },
            description: Resources.CommandFieldOption) { IsRequired = true };

        var definitionOption = new Option<string?>(new[] { "-d", "--definition" },
            description: Resources.CommandFieldOption);

        var definitionFileOption = new Option<string?>(new[] { "-f", "--definition-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(identityOption);
        AddOption(definitionOption);
        AddOption(definitionFileOption);
        AddOption(addressOption);
    }
}