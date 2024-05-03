using System.CommandLine;

namespace FlowSynx.Cli.Commands.Uninstall;

internal class UninstallCommand : BaseCommand<UninstallCommandOptions, UninstallCommandOptionsHandler>
{
    public UninstallCommand() : base("uninstall", "Uninstalling FlowSynx system, Dashboard, " +
                                                  "and Cli from the current user profile and machine")
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, 
            getDefaultValue :() => false, 
            description: "Force terminate FlowSynx system and Dashboard if they are running");

        AddOption(forceOption);
    }
}