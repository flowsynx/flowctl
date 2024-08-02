using System.CommandLine;
using FlowCtl.Commands.Dashboard.Version;

namespace FlowCtl.Commands.Dashboard;

internal class DashboardCommand : BaseCommand<DashboardCommandOptions, DashboardCommandOptionsHandler>
{
    public DashboardCommand() : base("dashboard", Resources.DashboardCommandDescription)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(addressOption);
        AddCommand(new DashboardVersionCommand());
    }
}