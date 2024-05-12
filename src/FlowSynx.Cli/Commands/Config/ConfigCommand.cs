using System.CommandLine;
using FlowSynx.Cli.Commands.Config.Add;
using FlowSynx.Cli.Commands.Config.Delete;
using FlowSynx.Cli.Commands.Config.Details;

namespace FlowSynx.Cli.Commands.Config;

internal class ConfigCommand : BaseCommand<ConfigCommandOptions, ConfigCommandOptionsHandler>
{
    public ConfigCommand() : base("config", "Manage configurations related to FlowSynx System")
    {
        var typeOption = new Option<string>(new []{ "-t", "--type" },
            description: "The type of configuration item");

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: "Formatting CLI output");

        AddOption(typeOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new AddConfigCommand());
        AddCommand(new DeleteConfigCommand());
        AddCommand(new DetailsConfigCommand());
    }
}