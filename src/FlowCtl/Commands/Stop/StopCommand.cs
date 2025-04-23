namespace FlowCtl.Commands.Stop;

internal class StopCommand : BaseCommand<StopCommandOptions, StopCommandOptionsHandler>
{
    public StopCommand() : base("stop", Resources.Commands_Stop_Description)
    {

    }
}