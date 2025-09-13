namespace FlowCtl.Commands.Console.Stop;

internal class CosnoleStopCommand : BaseCommand<ConsoleStopCommandOptions, ConsoleStopCommandOptionsHandler>
{
    public CosnoleStopCommand() : base("stop", Resources.Commands_StopConsole_Description)
    {

    }
}