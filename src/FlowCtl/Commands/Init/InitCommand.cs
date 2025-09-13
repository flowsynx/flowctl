using System.CommandLine;

namespace FlowCtl.Commands.Init;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", Resources.Commands_Init_Description)
    {
        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: Resources.Commands_Init_FlowSynxVersionOption);

        var consoleVersionOption = new Option<string?>("--console-version",
            description: Resources.Commands_Init_FlowSynxVersionOption);

        AddOption(flowSynxVersionOption);
        AddOption(consoleVersionOption);
    }
}