using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Update;

internal class UpdatePluginCommand : BaseCommand<UpdatePluginCommandOptions, UpdatePluginCommandOptionsHandler>
{
    public UpdatePluginCommand() : base("update", Resources.Commands_Plugins_UpdateDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.Commands_Plugins_Install_TypeOption) { IsRequired = true };

        var oldVersionOption = new Option<string>(new[] { "-o", "--old-version" },
            description: Resources.Commands_Plugins_OldVersionOption) { IsRequired = true };

        var newVersionOption = new Option<string>(new[] { "-n", "--new-version" },
            description: Resources.Commands_Plugins_NewVersionOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(typeOption);
        AddOption(oldVersionOption);
        AddOption(newVersionOption);
        AddOption(addressOption);
    }
}