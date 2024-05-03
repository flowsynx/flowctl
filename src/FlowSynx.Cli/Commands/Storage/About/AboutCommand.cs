using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.About;

internal class AboutCommand : BaseCommand<AboutCommandOptions, AboutCommandOptionsHandler>
{
    public AboutCommand() : base("about", "Get about information from the storage")
    {
        var pathOption = new Option<string?>(new[] { "-p", "--path" },
            description: "The storage path from which the about information is to be retrieved");

        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, 
            getDefaultValue: () => false,
            description: "Full numbers instead of human-readable");

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: "Formatting CLI output");

        AddOption(pathOption);
        AddOption(fullOption);
        AddOption(urlOption);
        AddOption(outputFormatOption);
    }
}