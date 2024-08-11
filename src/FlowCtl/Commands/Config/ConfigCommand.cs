using System.CommandLine;
using FlowCtl.Commands.Config.Add;
using FlowCtl.Commands.Config.Delete;
using FlowCtl.Commands.Config.Details;

namespace FlowCtl.Commands.Config;

internal class ConfigCommand : BaseCommand<ConfigCommandOptions, ConfigCommandOptionsHandler>
{
    public ConfigCommand() : base("config", Resources.ConfigCommandDescription)
    {
        var includeOption = new Option<string?>(new[] { "-i", "--include" },
            description: Resources.CommandIncludeOption);

        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" },
            description: Resources.CommandExcludeOption);

        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" },
            description: Resources.CommandMinAgeOption);

        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" },
            description: Resources.CommandMaxAgeOption);

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
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(caseSensitiveOption);
        AddOption(maxResultsOption);
        AddOption(sortingOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new AddConfigCommand());
        AddCommand(new DeleteConfigCommand());
        AddCommand(new DetailsConfigCommand());
    }
}