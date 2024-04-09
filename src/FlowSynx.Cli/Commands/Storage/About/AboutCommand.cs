using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.About;

internal class AboutCommand : BaseCommand<AboutCommandOptions, AboutCommandOptionsHandler>
{
    public AboutCommand() : base("about", "About storage")
    {
        var pathOption = new Option<string?>("--path", "The path to get about");
        var fullOption = new Option<bool?>("--full", getDefaultValue: () => false, "Full numbers instead of human-readable");
        var outputFormatOption = new Option<Output>("--output", getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(pathOption);
        AddOption(fullOption);
        AddOption(outputFormatOption);
    }
}