using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.About;

internal class AboutCommand : BaseCommand<AboutCommandOptions, AboutCommandOptionsHandler>
{
    public AboutCommand() : base("about", "About storage")
    {
        var pathOption = new Option<string?>(new[] { "-p", "--path" }, "The path to get about");
        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, getDefaultValue: () => false, "Full numbers instead of human-readable");
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");
        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(pathOption);
        AddOption(fullOption);
        AddOption(urlOption);
        AddOption(outputFormatOption);
    }
}