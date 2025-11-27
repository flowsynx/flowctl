using System.CommandLine;

namespace FlowCtl.Commands.Stop;

internal class StopCommand : BaseCommand<StopCommandOptions, StopCommandOptionsHandler>
{
    public StopCommand() : base("stop", Resources.Commands_Stop_Description)
    {
        var dockerOption = new Option<bool>("--docker",
            getDefaultValue: () => false,
            description: Resources.Commands_Stop_DockerDescription);

        AddOption(dockerOption);
    }
}
