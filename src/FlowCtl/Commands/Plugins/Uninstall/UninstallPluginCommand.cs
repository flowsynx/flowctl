using FlowCtl.Commands.Plugins.Install;
using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Uninstall;

internal class UninstallPluginCommand : BaseCommand<UninstallPluginCommandOptions, UninstallPluginCommandOptionsHandler>
{
    public UninstallPluginCommand() : base("uninstall", Resources.Commands_Plugins_UninstallDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.Commands_Plugins_TypeOption) { IsRequired = true };

        var versionOption = new Option<string>(new[] { "-v", "--version" },
            description: Resources.Commands_Plugins_VersionOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(typeOption);
        AddOption(versionOption);
        AddOption(addressOption);
    }
}