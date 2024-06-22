namespace FlowSynx.Cli.Commands.Stop;

internal class StopCommand : BaseCommand<StopCommandOptions, StopCommandOptionsHandler>
{
    public StopCommand() : base("stop", Resources.StopCommandDescription)
    {

    }
}