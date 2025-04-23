using System.CommandLine;
using FlowCtl.Commands.Plugins.Details;
using FlowCtl.Commands.Plugins.Install;
using FlowCtl.Commands.Plugins.Uninstall;
using FlowCtl.Commands.Plugins.Update;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", Resources.Commands_Plugins_Description)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new InstallPluginCommand());
        AddCommand(new UninstallPluginCommand());
        AddCommand(new PluginDetailsCommand());
        AddCommand(new UpdatePluginCommand());
    }
}