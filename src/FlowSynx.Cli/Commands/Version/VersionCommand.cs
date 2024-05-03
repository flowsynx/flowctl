using System.CommandLine;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", "Display the FlowSynx system, Dashboard, and Cli version")
    {
        var typeOption = new Option<bool>(new[] { "-f", "--full" }, 
            getDefaultValue: () => false,
            description: "Display full details about the running FlowSynx system");

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            description: "Formatting CLI output");

        AddOption(typeOption);
        AddOption(outputOption);
    }
}