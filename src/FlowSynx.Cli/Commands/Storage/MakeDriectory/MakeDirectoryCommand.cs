using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.MakeDriectory;

internal class MakeDirectoryCommand : BaseCommand<MakeDirectoryCommandOptions, MakeDirectoryCommandOptionsHandler>
{
    public MakeDirectoryCommand() : base("mkdir", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };

        AddOption(pathOption);
    }
}