using FlowSynx.IO.Compression;
using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Compress;

internal class CompressCommand : BaseCommand<CompressCommandOptions, CompressCommandOptionsHandler>
{
    public CompressCommand() : base("compress", Resources.CompressCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.CompressCommandPathOption) { IsRequired = true };

        var kindOption = new Option<ItemKind?>(new[] { "-k", "--kind" }, 
            getDefaultValue: () => ItemKind.FileAndDirectory,
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

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" }, 
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false,
            description: Resources.CommandRecurseOption);

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" },
            description: Resources.CommandMaxResultsOption);

        var hashingOption = new Option<bool?>(new[] { "+h", "--hashing" }, 
            getDefaultValue: () => false,
            description: Resources.CompressCommandHashingOption);

        var compressTypeOption = new Option<CompressType?>(new[] { "-t", "--compress-type" }, 
            getDefaultValue: () => CompressType.Zip,
            description: Resources.CompressCommandCompressTypeOption);

        var savePathOption = new Option<string>(new[] { "-s", "--save-to" },
            description: Resources.CompressCommandSaveToOption) { IsRequired = true };

        var overWriteOption = new Option<bool?>(new[] { "-w", "--overwrite" }, 
            getDefaultValue: () => false,
            description: Resources.CompressCommandOverwriteOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

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
        AddOption(addressOption);
    }
}