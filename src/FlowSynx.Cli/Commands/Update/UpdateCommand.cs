using System.CommandLine;

namespace FlowSynx.Cli.Commands.Update;

internal class UpdateCommand : BaseCommand<UpdateCommandOptions, UpdateCommandOptionsHandler>
{
    public UpdateCommand() : base("update", "Update FlowSynx system and Cli")
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, getDefaultValue: () => false, description: "Force terminate FlowSynx system if it is running");

        AddOption(forceOption);
    }
}