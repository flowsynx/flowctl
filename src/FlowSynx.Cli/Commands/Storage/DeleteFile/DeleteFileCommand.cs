using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.DeleteFile;

internal class DeleteFileCommand : BaseCommand<DeleteFileCommandOptions, DeleteFileCommandOptionsHandler>
{
    public DeleteFileCommand() : base("deletefile", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };

        AddOption(pathOption);
    }
}