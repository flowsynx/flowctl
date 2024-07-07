using System.CommandLine;

namespace FlowCtl.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", Resources.VersionCommandDescription)
    {
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            description: Resources.CommandOutputOption);
        
        AddOption(outputOption);
    }
}