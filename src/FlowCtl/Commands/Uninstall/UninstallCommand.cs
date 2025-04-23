using System.CommandLine;

namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommand : BaseCommand<UninstallCommandOptions, UninstallCommandOptionsHandler>
{
    public UninstallCommand() : base("uninstall", Resources.Commands_Uninstall_Description)
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, 
            getDefaultValue :() => false, 
            description: Resources.Commands_Uninstall_ForceTerminateOption);

        AddOption(forceOption);
    }
}