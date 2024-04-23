using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Copy;

internal class CopyCommand : BaseCommand<CopyCommandOptions, CopyCommandOptionsHandler>
{
    public CopyCommand() : base("copy", "List of entities regarding specific path")
    {
        var sourcePathOption = new Option<string>(new[] { "-s", "--source-path" }, "The path to get about") { IsRequired = true };
        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" }, "The path to get about") { IsRequired = true };
        var clearDestinationPathOption = new Option<bool?>(new[] { "-cp", "--clear-destination-path" }, getDefaultValue: () => false, "The maximum number of results to return [default: off]");
        var overWriteDataOption = new Option<bool?>(new[] { "-w", "--overWrite-data" }, getDefaultValue: () => false, "Formatting CLI output");
        var includeOption = new Option<string?>(new[] { "-i", "--include" }, "Include entities matching pattern");
        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" }, "Exclude entities matching pattern");
        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" }, "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" }, "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var minSizeOption = new Option<string?>(new[] { "-ms", "--min-size" }, "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var maxSizeOption = new Option<string?>(new[] { "+ms", "--max-size" }, "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, getDefaultValue: () => false, "Ignore or apply case sensitive in filters");
        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, getDefaultValue: () => false, "Apply recursion on filtering entities in the specified path");
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(sourcePathOption);
        AddOption(destinationPathOption);
        AddOption(clearDestinationPathOption);
        AddOption(overWriteDataOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(urlOption);
    }
}