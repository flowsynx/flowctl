namespace FlowCtl.Commands.Run;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", Resources.Commands_Run_Description)
    {

    }
}