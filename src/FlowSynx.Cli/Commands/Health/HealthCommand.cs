using System.CommandLine;

namespace FlowSynx.Cli.Commands.Health;

internal class HealthCommand : BaseCommand<HealthCommandOptions, HealthCommandOptionsHandler>
{
    public HealthCommand() : base("health", "Display the health status of FlowSynx System")
    {
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(urlOption);
        AddOption(outputOption);
    }
}