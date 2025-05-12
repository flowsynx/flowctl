using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Config.Details;

internal class DetailsConfigCommand : BaseCommand<DetailsConfigCommandOptions, DetailsConfigCommandOptionsHandler>
{
    public DetailsConfigCommand() : base("details", Resources.Commands_DetailsConfig_Description)
    {
        var configIdOption = new Option<string>(new[] { "-c", "--config-id" },
            description: Resources.Commands_DetailsConfig_IdentityOption) { IsRequired = true };

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(configIdOption);
        AddOption(outputFormatOption);
    }
}