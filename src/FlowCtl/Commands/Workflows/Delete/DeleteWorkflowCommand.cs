using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Delete;

internal class DeleteWorkflowCommand : BaseCommand<DeleteWorkflowCommandOptions, DeleteWorkflowCommandOptionsHandler>
{
    public DeleteWorkflowCommand() : base("delete", Resources.ConnectorDetailsCommandDescription)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(identityOption);
        AddOption(addressOption);
    }
}