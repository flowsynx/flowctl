using System.CommandLine;
using FlowCtl.Commands.Config.Add;
using FlowCtl.Commands.Config.Delete;
using FlowCtl.Commands.Config.Details;

namespace FlowCtl.Commands.Config;

internal class ConfigCommand : BaseCommand<ConfigCommandOptions, ConfigCommandOptionsHandler>
{
    public ConfigCommand() : base("config", Resources.ConfigCommandDescription)
    {
        var fieldsOption = new Option<string[]?>(new[] { "-fd", "--fields" },
            getDefaultValue: Array.Empty<string>,
            description: Resources.CommandIncludeOption);

        var filterOption = new Option<string?>(new[] { "-f", "--filter" },
            getDefaultValue: () => string.Empty,
            description: Resources.CommandExcludeOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var sortOption = new Option<string?>(new[] { "-s", "--sort" },
            getDefaultValue: () => string.Empty,
            description: Resources.CommandSortingOption);

        var limitOption = new Option<string?>(new[] { "-l", "--limit" },
            getDefaultValue: () => string.Empty,
            description: Resources.LogsCommandLogLevelOption);

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

        AddCommand(new AddConfigCommand());
        AddCommand(new DeleteConfigCommand());
        AddCommand(new DetailsConfigCommand());
    }
}