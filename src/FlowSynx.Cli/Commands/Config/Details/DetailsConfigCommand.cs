using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Details;

internal class DetailsConfigCommand : BaseCommand<DetailsConfigCommandOptions, DetailsConfigCommandOptionsHandler>
{
    public DetailsConfigCommand() : base("details", "Get details about configuration section")
    {
        var nameOption = new Option<string>("--name", "The configuration section name") { IsRequired = true };
        var outputFormatOption = new Option<Output>("--output", getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(nameOption);
        AddOption(outputFormatOption);
    }
}