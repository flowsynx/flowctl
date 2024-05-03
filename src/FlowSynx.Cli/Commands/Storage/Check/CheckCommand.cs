using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Check;

internal class CheckCommand : BaseCommand<CheckCommandOptions, CheckCommandOptionsHandler>
{
    public CheckCommand() : base("check", "Checks the files in the source and destination match")
    {
        var sourcePathOption = new Option<string>(new[] { "-s", "--source-path" },
            description: "The storage source's path that to be checked") { IsRequired = true };

        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" },
            description: "The storage destination's path to be checked against") { IsRequired = true };

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

        var checkSizeOption = new Option<bool?>(new[] { "-cks", "--check-size" }, 
            getDefaultValue: () => false,
            description: "Compare and check the sizes of entities");

        var checkHashOption = new Option<bool?>(new[] { "-ckh", "--check-hash" }, 
            getDefaultValue: () => false,
            description: "Compare and check the hashes of entities");

        var oneWayOption = new Option<bool?>(new[] { "-w", "--oneway" }, 
            getDefaultValue: () => false,
            description: "Only check that entities in the source match the entities in the destination, not the other way around");

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            "Formatting CLI output");

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
        AddOption(urlOption);
        AddOption(outputOption);
    }
}