using FlowCtl.Core.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", Resources.VersionCommandDescription)
    {
        var outputOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json, 
            description: Resources.CommandOutputOption);
        
        AddOption(outputOption);
    }
}