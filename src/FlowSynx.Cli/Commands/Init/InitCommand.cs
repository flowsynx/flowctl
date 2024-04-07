namespace FlowSynx.Cli.Commands.Init;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", "Install and initialize FlowSynx system on the current user profile") {}
}