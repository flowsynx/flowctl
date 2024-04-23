using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Details;

internal class DetailsConfigCommand : BaseCommand<DetailsConfigCommandOptions, DetailsConfigCommandOptionsHandler>
{
    public DetailsConfigCommand() : base("details", "Get details about configuration section")
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" }, "The configuration section name") { IsRequired = true };
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");
        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(nameOption);
        AddOption(urlOption);
        AddOption(outputFormatOption);
    }
}