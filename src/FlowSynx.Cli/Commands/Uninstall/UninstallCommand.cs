using System.CommandLine;

namespace FlowSynx.Cli.Commands.Uninstall;

internal class UninstallCommand : BaseCommand<UninstallCommandOptions, UninstallCommandOptionsHandler>
{
    public UninstallCommand() : base("uninstall", "Uninstalling FlowSynx system and Cli from the current user profile and machine")
    {
        var forceOption = new Option<bool>("--force", getDefaultValue :() => false, description: "Force terminate FlowSynx system if it is running");

        AddOption(forceOption);
    }
}