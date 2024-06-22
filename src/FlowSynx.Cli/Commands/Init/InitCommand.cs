using System.CommandLine;

namespace FlowSynx.Cli.Commands.Init;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", Resources.InitCommandDescription)
    {
        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: Resources.CommandFlowSynxVersionOption);

        var dashboardVersionOption = new Option<string?>("--dashboard-version",
            description: Resources.CommandDashboardVersionOption);

        AddOption(flowSynxVersionOption);
        AddOption(dashboardVersionOption);
    }
}