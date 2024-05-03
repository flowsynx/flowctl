﻿using System.CommandLine;

namespace FlowSynx.Cli.Commands.Init;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", "Install and initialize FlowSynx system and Dashboard on the current user profile")
    {
        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: "The version of the FlowSynx system to install, for example: 0.1.0");

        var dashboardVersionOption = new Option<string?>("--dashboard-version",
            description: "The version of the FlowSynx dashboard to install, for example: 0.1.0");

        AddOption(flowSynxVersionOption);
        AddOption(dashboardVersionOption);
    }
}