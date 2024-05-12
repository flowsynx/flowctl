using System.CommandLine;

namespace FlowSynx.Cli.Commands.Health;

internal class HealthCommand : BaseCommand<HealthCommandOptions, HealthCommandOptionsHandler>
{
    public HealthCommand() : base("health", "Display the health status of FlowSynx System")
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: "Formatting CLI output");

        AddOption(addressOption);
        AddOption(outputOption);
    }
}