using System.CommandLine;

namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommand : BaseCommand<UninstallCommandOptions, UninstallCommandOptionsHandler>
{
    public UninstallCommand() : base("uninstall", Resources.Commands_Uninstall_Description)
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, 
            getDefaultValue :() => false, 
            description: Resources.Commands_Uninstall_ForceTerminateOption);

        var dockerOption = new Option<bool>("--docker",
            getDefaultValue: () => false,
            description: Resources.Commands_Uninstall_DockerOption);

        var removeDataOption = new Option<bool>("--remove-data",
            getDefaultValue: () => false,
            description: Resources.Commands_Uninstall_RemoveDataOption);

        AddOption(forceOption);
        AddOption(dockerOption);
        AddOption(removeDataOption);
    }
}
