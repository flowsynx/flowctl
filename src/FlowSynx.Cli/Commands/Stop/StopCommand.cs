namespace FlowSynx.Cli.Commands.Stop;

internal class StopCommand : BaseCommand<StopCommandOptions, StopCommandOptionsHandler>
{
    public StopCommand() : base("stop", "Stop the FlowSynx system which running on the current user profile") {}
}