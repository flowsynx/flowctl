using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Execute;

internal class ExecuteWorkflowCommand : BaseCommand<ExecuteWorkflowCommandOptions, ExecuteWorkflowCommandOptionsHandler>
{
    public ExecuteWorkflowCommand() : base("execute", Resources.ConnectorDetailsCommandDescription)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(identityOption);
        AddOption(addressOption);
    }
}