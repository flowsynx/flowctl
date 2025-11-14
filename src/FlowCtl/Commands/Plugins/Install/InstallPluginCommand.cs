using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Install;

internal class InstallPluginCommand : BaseCommand<InstallPluginCommandOptions, InstallPluginCommandOptionsHandler>
{
    public InstallPluginCommand() : base("install", Resources.Commands_Plugins_InstallDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.Commands_Plugins_Install_TypeOption) { IsRequired = true };

        var versionOption = new Option<string?>(new[] { "-v", "--version" },
            description: Resources.Commands_Plugins_VersionOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(typeOption);
        AddOption(versionOption);
        AddOption(addressOption);
    }
}
