using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.List;

internal class ListCommand : BaseCommand<ListCommandOptions, ListCommandOptionsHandler>
{
    public ListCommand() : base("list", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, 
            description: "The storage path from which the entities list are to be retrieved") { IsRequired = true };

        var kindOption = new Option<string?>(new[] { "-k", "--kind" }, 
            getDefaultValue: () => nameof(ItemKind.FileAndDirectory),
            description: "Kind of entity. Valid values are File, Directory, and FileAndDirectory");

        var includeOption = new Option<string?>(new[] { "-i", "--include" }, 
            description: "Include entities matching pattern");

        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" }, 
            description: "Exclude entities matching pattern");

        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" }, 
            description: "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");

        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" }, 
            description: "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");

        var minSizeOption = new Option<string?>(new[] { "-ms", "--min-size" }, 
            description: "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");

        var maxSizeOption = new Option<string?>(new[] { "+ms", "--max-size" },
            description: "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");

        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, 
            getDefaultValue: () => false, 
            description: "Full numbers instead of human-readable");

        var sortingOption = new Option<string?>(new[] { "-so", "--sorting" }, 
            description: "Sorting entities based on field name and ascending and descending. Like Property ASC, Property2 DESC [default: off]");

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, 
            getDefaultValue: () => false, 
            description: "Ignore or apply case sensitive in filters");

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false, 
            description: "Apply recursion on filtering entities in the specified path");

        var hashingOption = new Option<bool?>(new[] { "+h", "--hashing" }, 
            getDefaultValue: () => false, 
            description: "Display hashing content in response data");

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" }, 
            description: "The maximum number of results to return [default: off]");

        var showMetadataOption = new Option<bool?>(new[] { "-sm", "--show-metadata" }, 
            getDefaultValue: () => false, 
            description: "Display entities metadata in response data");

        var urlOption = new Option<string?>(new[] { "-u", "--url" }, 
            description: "The address that specify a URL to connect on remote FlowSynx system");

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: "Formatting CLI output");

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
        AddOption(urlOption);
        AddOption(outputOption);
    }
}