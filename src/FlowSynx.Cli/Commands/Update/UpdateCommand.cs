using System.CommandLine;

namespace FlowSynx.Cli.Commands.Update;

internal class UpdateCommand : BaseCommand<UpdateCommandOptions, UpdateCommandOptionsHandler>
{
    public UpdateCommand() : base("update", "Update FlowSynx system and Cli")
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, getDefaultValue: () => false, description: "Force terminate FlowSynx system if it is running");
        var flowsynxVersionOption = new Option<string?>("--flowsynx-version", "The version of the FlowSynx system to install, for example: 0.1.0");
        var dashboardVersionOption = new Option<string?>("--dashboard-version", "The version of the FlowSynx dashboard to install, for example: 0.1.0");

        AddOption(forceOption);
        AddOption(flowsynxVersionOption);
        AddOption(dashboardVersionOption);
    }
}