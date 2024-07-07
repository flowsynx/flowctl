using System.CommandLine;

namespace FlowCtl.Commands.Update;

internal class UpdateCommand : BaseCommand<UpdateCommandOptions, UpdateCommandOptionsHandler>
{
    public UpdateCommand() : base("update", Resources.UpdateCommandDescription)
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, 
            getDefaultValue: () => false, 
            description: Resources.CommandForceOption);

        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: Resources.CommandFlowSynxVersionOption);

        var dashboardVersionOption = new Option<string?>("--dashboard-version",
            description: Resources.CommandDashboardVersionOption);

        AddOption(forceOption);
        AddOption(flowSynxVersionOption);
        AddOption(dashboardVersionOption);
    }
}