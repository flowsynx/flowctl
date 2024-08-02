using System.CommandLine;

namespace FlowCtl.Commands.Dashboard.Version;

internal class DashboardVersionCommand : BaseCommand<DashboardVersionCommandOptions, DashboardVersionCommandOptionsHandler>
{
    public DashboardVersionCommand() : base("version", Resources.DashboardVersionCommandDescription)
    {
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            description: Resources.CommandOutputOption);
        
        AddOption(outputOption);
    }
}