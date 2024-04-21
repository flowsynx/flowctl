using FlowSynx.IO.Compression;
using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Compress;

internal class CompressCommand : BaseCommand<CompressCommandOptions, CompressCommandOptionsHandler>
{
    public CompressCommand() : base("compress", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };
        var kindOption = new Option<ItemKind?>(new[] { "-k", "--kind" }, getDefaultValue: () => ItemKind.FileAndDirectory, "Should apply format for byte size. Valid values are File, Directory, and FileAndDirectory.");
        var includeOption = new Option<string?>(new[] { "-i", "--include" }, "Include entities matching pattern");
        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" }, "Exclude entities matching pattern");
        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" }, "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" }, "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var minSizeOption = new Option<string?>(new[] { "-ms", "--min-size" }, "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var maxSizeOption = new Option<string?>(new[] { "+ms", "--max-size" }, "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, getDefaultValue: () => false, "Ignore or apply case sensitive in filters");
        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, getDefaultValue: () => false, "Apply recursion on filtering entities in the specified path");
        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" }, "The maximum number of results to return [default: off]");
        var hashingOption = new Option<bool?>(new[] { "-h", "--hashing" }, getDefaultValue: () => false, "Display hashing content in response data [default: off]");
        var compressTypeOption = new Option<CompressType?>(new[] { "-t", "--compress-type" }, getDefaultValue: () => CompressType.Zip, "Display metadata in response data [default: off]");
        var savePathOption = new Option<string>(new[] { "-s", "--save-to" }, "The path to get about") { IsRequired = true };
        var overWriteOption = new Option<bool?>(new[] { "-w", "--overwrite" }, getDefaultValue: () => false, "The path to get about");

        AddOption(pathOption);
        AddOption(kindOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(hashingOption);
        AddOption(maxResultsOption);
        AddOption(compressTypeOption);
        AddOption(savePathOption);
        AddOption(overWriteOption);
    }
}