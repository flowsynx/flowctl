using System.CommandLine;

namespace FlowCtl.Commands.Dashboard.Version;

internal class DashboardVersionCommand : BaseCommand<DashboardVersionCommandOptions, DashboardVersionCommandOptionsHandler>
{
    public DashboardVersionCommand() : base("version", Resources.VersionCommandDescription)
    {
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            description: Resources.CommandOutputOption);
        
        AddOption(outputOption);
    }
}