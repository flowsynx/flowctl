using System.CommandLine;

namespace FlowSynx.Cli.Commands.Dashboard;

internal class DashboardCommand : BaseCommand<DashboardCommandOptions, DashboardCommandOptionsHandler>
{
    public DashboardCommand() : base("dashboard", "Run and execute the FlowSynx dashboard")
    {
        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(urlOption);
    }
}