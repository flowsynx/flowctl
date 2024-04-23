using System.CommandLine;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", "Display the FlowSynx system and Cli version")
    {
        var typeOption = new Option<bool>(new[] { "-f", "--full" }, getDefaultValue: () => false, "Display full details about the running FlowSynx system");
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(typeOption);
        AddOption(urlOption);
        AddOption(outputOption);
    }
}