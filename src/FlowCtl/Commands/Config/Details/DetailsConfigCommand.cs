using FlowCtl.Core.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Config.Details;

internal class DetailsConfigCommand : BaseCommand<DetailsConfigCommandOptions, DetailsConfigCommandOptionsHandler>
{
    public DetailsConfigCommand() : base("details", Resources.DetailsConfigCommandDescription)
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" },
            description: Resources.DetailsConfigCommandNameOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputFormatOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);

        AddOption(nameOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}