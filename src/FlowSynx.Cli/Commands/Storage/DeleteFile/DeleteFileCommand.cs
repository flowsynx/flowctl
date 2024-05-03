using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.DeleteFile;

internal class DeleteFileCommand : BaseCommand<DeleteFileCommandOptions, DeleteFileCommandOptionsHandler>
{
    public DeleteFileCommand() : base("deletefile", "Delete entity from specific storage path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description:"The entity's path that are going to be deleted") { IsRequired = true };

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(urlOption);
    }
}