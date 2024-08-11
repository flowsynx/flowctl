using System.CommandLine;
using FlowCtl.Commands.Plugins.Details;

namespace FlowCtl.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", Resources.PluginsCommandDescription)
    {
        var includeOption = new Option<string?>(new[] { "-i", "--include" },
            description: Resources.CommandIncludeOption);

        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" },
            description: Resources.CommandExcludeOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" },
            description: Resources.CommandMaxResultsOption);

        var sortingOption = new Option<string?>(new[] { "-so", "--sorting" },
            description: Resources.CommandSortingOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" },
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(caseSensitiveOption);
        AddOption(maxResultsOption);
        AddOption(sortingOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new PluginDetailsCommand());
    }
}