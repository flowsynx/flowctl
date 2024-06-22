using System.CommandLine;

namespace FlowSynx.Cli.Commands.Dashboard;

internal class DashboardCommand : BaseCommand<DashboardCommandOptions, DashboardCommandOptionsHandler>
{
    public DashboardCommand() : base("dashboard", Resources.DashboardCommandDescription)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(addressOption);
    }
}