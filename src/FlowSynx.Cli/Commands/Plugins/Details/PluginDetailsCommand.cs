using System.CommandLine;

namespace FlowSynx.Cli.Commands.Plugins.Details;

internal class PluginDetailsCommand : BaseCommand<PluginDetailsCommandOptions, PluginDetailsCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", "Display details about a particular plugin")
    {
        var nameOption = new Option<Guid>(new[] { "-i", "--id" }, "The identifier (Id) of the plugin") { IsRequired = true };
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");
        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(nameOption);
        AddOption(urlOption);
        AddOption(outputFormatOption);
    }
}