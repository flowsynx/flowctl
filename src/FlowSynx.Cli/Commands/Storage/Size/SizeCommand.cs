using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Size;

internal class SizeCommand : BaseCommand<SizeCommandOptions, SizeCommandOptionsHandler>
{
    public SizeCommand() : base("size", Resources.SizeCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.SizeCommandPathOption) { IsRequired = true };

        var kindOption = new Option<string?>(new[] { "-k", "--kind" }, 
            getDefaultValue: () => nameof(ItemKind.FileAndDirectory),
            description: Resources.CommandKindOption);

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

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, 
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false,
            description: Resources.CommandRecurseOption);

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" },
            description: Resources.CommandMaxResultsOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(pathOption);
        AddOption(kindOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(fullOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(maxResultsOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}