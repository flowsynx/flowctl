﻿using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Move;

internal class MoveCommand : BaseCommand<MoveCommandOptions, MoveCommandOptionsHandler>
{
    public MoveCommand() : base("move", "Move entities from source storage to the destination.")
    {
        var sourcePathOption = new Option<string>(new[] { "-s", "--source-path" },
            description: "The storage path from which the entities are to be moved") { IsRequired = true };

        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" },
            description: "The storage path to which the entities are to be moved") { IsRequired = true };

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

        var clearDestinationPathOption = new Option<bool?>(new[] { "-cp", "--clear-destination-path" }, 
            getDefaultValue: () => false,
            description: "Clearing all entities and other things in the destination path before starting the copy operation");

        var createEmptyDirectoriesOption = new Option<bool?>(new[] { "-cd", "--create-empty-directories" }, 
            getDefaultValue: () => false,
            description: "Create empty source directories on destination after move");

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

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
        AddOption(createEmptyDirectoriesOption);
        AddOption(addressOption);
    }
}