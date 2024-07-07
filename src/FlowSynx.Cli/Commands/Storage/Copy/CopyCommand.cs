using System.CommandLine;

namespace FlowCtl.Commands.Storage.Copy;

internal class CopyCommand : BaseCommand<CopyCommandOptions, CopyCommandOptionsHandler>
{
    public CopyCommand() : base("copy", Resources.CopyCommandDescription)
    {
        var sourcePathOption = new Option<string>(new[] { "-s", "--source-path" },
            description: Resources.CopyCommandSourcePathOption) { IsRequired = true };

        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" },
            description: Resources.CopyCommandDestinationPathOption) { IsRequired = true };
        
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

        var clearDestinationPathOption = new Option<bool?>(new[] { "-cp", "--clear-destination-path" },
            getDefaultValue: () => false,
            description: Resources.CopyCommandClearDestinationPathOption);

        var overWriteDataOption = new Option<bool?>(new[] { "-w", "--overwrite-data" },
            getDefaultValue: () => false,
            description: Resources.CopyCommandOverwriteDataOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(sourcePathOption);
        AddOption(destinationPathOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(clearDestinationPathOption);
        AddOption(overWriteDataOption);
        AddOption(addressOption);
    }
}