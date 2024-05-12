using System.CommandLine;

namespace FlowSynx.Cli.Commands.Dashboard;

internal class DashboardCommand : BaseCommand<DashboardCommandOptions, DashboardCommandOptionsHandler>
{
    public DashboardCommand() : base("dashboard", "Run and execute the FlowSynx dashboard")
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        AddOption(addressOption);
    }
}