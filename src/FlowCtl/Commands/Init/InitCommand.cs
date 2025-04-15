using System.CommandLine;

namespace FlowCtl.Commands.Init;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", Resources.InitCommandDescription)
    {
        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: Resources.CommandFlowSynxVersionOption);

        AddOption(flowSynxVersionOption);
    }
}