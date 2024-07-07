using System.CommandLine;

namespace FlowCtl.Commands.Storage.Check;

internal class CheckCommand : BaseCommand<CheckCommandOptions, CheckCommandOptionsHandler>
{
    public CheckCommand() : base("check", Resources.CheckCommandDescription)
    {
        var sourcePathOption = new Option<string>(new[] { "-s", "--source-path" },
            description: Resources.CheckCommandSourcePathOption) { IsRequired = true };

        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" },
            description: Resources.CheckCommandDestinationPathOption) { IsRequired = true };

        var includeOption = new Option<string?>(new[] { "-i", "--include" },
            description: Resources.CommandIncludeOption);

        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" },
            description: Resources.CommandExcludeOption);

        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" },
            description: Resources.CommandMinAgeOption);

        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" },
            description: Resources.CommandMaxAgeOption);

        var minSizeOption = new Option<string?>(new[] { "-ms", "--min-size" },
            description: Resources.CommandMinSizeOption);

        var maxSizeOption = new Option<string?>(new[] { "+ms", "--max-size" },
            description: Resources.CommandMaxSizeOption);

        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, 
            getDefaultValue: () => false,
            description: Resources.CommandFullOption);

        var sortingOption = new Option<string?>(new[] { "-so", "--sorting" },
            description: Resources.CommandSortingOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, 
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false,
            description: Resources.CommandRecurseOption);

        var checkSizeOption = new Option<bool?>(new[] { "-cks", "--check-size" }, 
            getDefaultValue: () => false,
            description: Resources.CheckCommandCheckSizeOption);

        var checkHashOption = new Option<bool?>(new[] { "-ckh", "--check-hash" }, 
            getDefaultValue: () => false,
            description: Resources.CheckCommandCheckHashOption);

        var oneWayOption = new Option<bool?>(new[] { "-w", "--oneway" }, 
            getDefaultValue: () => false,
            description: Resources.CheckCommandOneWayOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            Resources.CommandOutputOption);

        AddOption(sourcePathOption);
        AddOption(destinationPathOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(fullOption);
        AddOption(sortingOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(checkSizeOption);
        AddOption(checkHashOption);
        AddOption(oneWayOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}