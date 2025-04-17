using System.CommandLine;

namespace FlowCtl.Commands.Logout;

internal class LogoutCommand : BaseCommand<LogoutCommandOptions, LogoutCommandOptionsHandler>
{
    public LogoutCommand() : base("logout", Resources.HealthCommandDescription)
    {

    }
}