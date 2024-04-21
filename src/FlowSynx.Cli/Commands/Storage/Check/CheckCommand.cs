using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Check;

internal class CheckCommand : BaseCommand<CheckCommandOptions, CheckCommandOptionsHandler>
{
    public CheckCommand() : base("check", "List of entities regarding specific path")
    {
        var sourcePathOption = new Option<string>(new[] { "-s", "--source-path" }, "The path to get about") { IsRequired = true };
        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" }, "The path to get about") { IsRequired = true };
        var includeOption = new Option<string?>(new[] { "-i", "--include" }, "Include entities matching pattern");
        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" }, "Exclude entities matching pattern");
        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" }, "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" }, "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var minSizeOption = new Option<string?>(new[] { "-ms", "--min-size" }, "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var maxSizeOption = new Option<string?>(new[] { "+ms", "--max-size" }, "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, getDefaultValue: () => false, "Full numbers instead of human-readable");
        var sortingOption = new Option<string?>(new[] { "-s", "--sorting" }, "Sorting entities based on field name and ascending and descending. Like Property ASC, Property2 DESC [default: off]");
        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, getDefaultValue: () => false, "Ignore or apply case sensitive in filters");
        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, getDefaultValue: () => false, "Apply recursion on filtering entities in the specified path");
        var checkSizeOption = new Option<bool?>(new[] { "-cks", "--check-size" }, getDefaultValue: () => false, "Display hashing content in response data [default: off]");
        var checkHashOption = new Option<bool?>(new[] { "-ckh", "--check-hash" }, getDefaultValue: () => false, "The maximum number of results to return [default: off]");
        var oneWayOption = new Option<bool?>(new[] { "-w", "--oneway" }, getDefaultValue: () => false, "Display metadata in response data [default: off]");
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

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
        AddOption(outputOption);
    }
}