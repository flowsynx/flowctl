using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Delete;

internal class DeleteCommand : BaseCommand<DeleteCommandOptions, DeleteCommandOptionsHandler>
{
    public DeleteCommand() : base("delete", "Delete entities from specific storage path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The storage path from which the entities are to be deleted") { IsRequired = true };

        var includeOption = new Option<string?>("--include",
            description: "Include entities matching pattern");

        var excludeOption = new Option<string?>("--exclude",
            description: "Exclude entities matching pattern");

        var minAgeOption = new Option<string?>("--min-age",
            description: "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");

        var maxAgeOption = new Option<string?>("--max-age",
            description: "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");

        var minSizeOption = new Option<string?>("--min-size",
            description: "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");

        var maxSizeOption = new Option<string?>("--max-size", 
            description: "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");

        var caseSensitiveOption = new Option<bool?>("--case-sensitive", 
            getDefaultValue: () => false, 
            description: "Ignore or apply case sensitive in filters");

        var recurseOption = new Option<bool?>(new[] { "-r", "--recurse" }, 
            getDefaultValue: () => false, 
            description: "Apply recursion on filtering entities in the specified path");

        var urlOption = new Option<string?>(new[] { "-u", "--url" }, 
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
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