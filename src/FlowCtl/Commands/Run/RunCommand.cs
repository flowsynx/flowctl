﻿using System.CommandLine;

namespace FlowCtl.Commands.Run;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", Resources.Commands_Run_Description)
    {
        var backgroundOption = new Option<bool>(new[] { "-b", "--background" },
            getDefaultValue: () => false,
            description: Resources.Command_Run_BackgroundOption);

        AddOption(backgroundOption);
    }
}