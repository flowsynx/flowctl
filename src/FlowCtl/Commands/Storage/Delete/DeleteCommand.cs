using System.CommandLine;

namespace FlowCtl.Commands.Storage.Delete;

internal class DeleteCommand : BaseCommand<DeleteCommandOptions, DeleteCommandOptionsHandler>
{
    public DeleteCommand() : base("delete", Resources.DeleteCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.DeleteCommandPathOption) { IsRequired = true };

        var includeOption = new Option<string?>("--include",
            description: Resources.CommandIncludeOption);

        var excludeOption = new Option<string?>("--exclude",
            description: Resources.CommandExcludeOption);

        var minAgeOption = new Option<string?>("--min-age",
            description: Resources.CommandMinAgeOption);

        var maxAgeOption = new Option<string?>("--max-age",
            description: Resources.CommandMaxAgeOption);

        var minSizeOption = new Option<string?>("--min-size",
            description: Resources.CommandMinSizeOption);

        var maxSizeOption = new Option<string?>("--max-size", 
            description: Resources.CommandMaxSizeOption);

        var caseSensitiveOption = new Option<bool?>("--case-sensitive", 
            getDefaultValue: () => false, 
            description: Resources.CommandCaseSensitiveOption);

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false, 
            description: Resources.CommandRecurseOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(pathOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
        AddOption(addressOption);
    }
}