using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", Resources.Commands_Version_Description)
    {
        var outputOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json, 
            description: Resources.Commands_Output_Format);
        
        AddOption(outputOption);
    }
}