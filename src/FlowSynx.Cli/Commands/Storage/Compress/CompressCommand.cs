using FlowSynx.IO.Compression;
using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Compress;

internal class CompressCommand : BaseCommand<CompressCommandOptions, CompressCommandOptionsHandler>
{
    public CompressCommand() : base("compress", "Compress of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The storage path to be compressed") { IsRequired = true };

        var kindOption = new Option<ItemKind?>(new[] { "-k", "--kind" }, 
            getDefaultValue: () => ItemKind.FileAndDirectory,
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

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, 
            getDefaultValue: () => false,
            description: "Ignore or apply case sensitive in filters");

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false,
            description: "Apply recursion on filtering entities in the specified path");

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" },
            description: "The maximum number of results to return [default: off]");

        var hashingOption = new Option<bool?>(new[] { "+h", "--hashing" }, 
            getDefaultValue: () => false,
            description: "Generate and display hashing content in response data");

        var compressTypeOption = new Option<CompressType?>(new[] { "-t", "--compress-type" }, 
            getDefaultValue: () => CompressType.Zip,
            description: "Type of compression. Valid values are Zip, GZip, Tar");

        var savePathOption = new Option<string>(new[] { "-s", "--save-to" },
            description: "The path where the streaming data are going to be saved") { IsRequired = true };

        var overWriteOption = new Option<bool?>(new[] { "-w", "--overwrite" }, 
            getDefaultValue: () => false,
            description: "Overwriting the streaming data if the file (save-to) exists");

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

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
        AddOption(urlOption);
    }
}