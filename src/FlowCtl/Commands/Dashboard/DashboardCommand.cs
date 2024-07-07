using System.CommandLine;

namespace FlowCtl.Commands.Dashboard;

internal class DashboardCommand : BaseCommand<DashboardCommandOptions, DashboardCommandOptionsHandler>
{
    public DashboardCommand() : base("dashboard", Resources.DashboardCommandDescription)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(addressOption);
    }
}