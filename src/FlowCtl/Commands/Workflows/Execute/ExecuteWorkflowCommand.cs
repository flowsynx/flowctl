using System.CommandLine;

namespace FlowCtl.Commands.Workflows.Execute;

internal class ExecuteWorkflowCommand : BaseCommand<ExecuteWorkflowCommandOptions, ExecuteWorkflowCommandOptionsHandler>
{
    public ExecuteWorkflowCommand() : base("execute", Resources.Commands_Workflows_ExecuteDescription)
    {
        var identityOption = new Option<string>(new[] { "-i", "--id" },
            description: Resources.Commands_Workflows_ExecuteIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(identityOption);
        AddOption(addressOption);
    }
}