using System.CommandLine;

namespace FlowCtl.Commands.Update;

internal class UpdateCommand : BaseCommand<UpdateCommandOptions, UpdateCommandOptionsHandler>
{
    public UpdateCommand() : base("update", Resources.Commands_Update_Description)
    {
        var forceOption = new Option<bool>(new[] { "-f", "--force" }, 
            getDefaultValue: () => false, 
            description: Resources.Commands_Update_ForceTerminateOption);

        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: Resources.Commands_Update_FlowSynxVersionOption);

        AddOption(forceOption);
        AddOption(flowSynxVersionOption);
    }
}