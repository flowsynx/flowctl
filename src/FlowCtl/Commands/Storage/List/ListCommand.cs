using System.CommandLine;

namespace FlowCtl.Commands.Storage.List;

internal class ListCommand : BaseCommand<ListCommandOptions, ListCommandOptionsHandler>
{
    public ListCommand() : base("list", Resources.ListCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, 
            description: Resources.ListCommandPathOption) { IsRequired = true };

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

        var sortingOption = new Option<string?>(new[] { "-so", "--sorting" }, 
            description: Resources.CommandSortingOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, 
            getDefaultValue: () => false, 
            description: Resources.CommandCaseSensitiveOption);

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false, 
            description: Resources.CommandRecurseOption);

        var hashingOption = new Option<bool?>(new[] { "+h", "--hashing" }, 
            getDefaultValue: () => false, 
            description: Resources.ListCommandHashingOption);

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" }, 
            description: Resources.CommandMaxResultsOption);

        var showMetadataOption = new Option<bool?>(new[] { "-im", "--include-metadata" }, 
            getDefaultValue: () => false, 
            description: Resources.ListCommandDisplayEntitiesMetadataOption);

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
        AddOption(sortingOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(hashingOption);
        AddOption(maxResultsOption);
        AddOption(showMetadataOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}