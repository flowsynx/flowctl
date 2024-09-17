using System.CommandLine;
using FlowCtl.Commands.Plugins.Details;

namespace FlowCtl.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", Resources.PluginsCommandDescription)
    {
        var fieldsOption = new Option<string[]?>(new[] { "-fd", "--fields" },
            description: Resources.CommandFieldOption);

        var filterOption = new Option<string?>(new[] { "-f", "--filter" },
            getDefaultValue: () => string.Empty,
            description: Resources.CommandFilterOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-c", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var sortOption = new Option<string?>(new[] { "-s", "--sort" },
            description: Resources.CommandSortOption);

        var limitOption = new Option<string?>(new[] { "-l", "--limit" },
            description: Resources.CommandLimitOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" },
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(fieldsOption);
        AddOption(filterOption);
        AddOption(caseSensitiveOption);
        AddOption(sortOption);
        AddOption(limitOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new PluginDetailsCommand());
    }
}