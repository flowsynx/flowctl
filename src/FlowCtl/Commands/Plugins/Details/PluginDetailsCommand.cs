﻿using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Details;

internal class PluginDetailsCommand : BaseCommand<PluginDetailsCommandOptions, PluginDetailsCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", Resources.Commands_Plugins_DetailsDescription)
    {
        var pluginIdOption = new Option<string>(new[] { "-p", "--plugin-id" },
            description: Resources.Commands_Plugins_DetailsIdentityOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(pluginIdOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}