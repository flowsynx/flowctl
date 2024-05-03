using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Write;

internal class WriteCommand : BaseCommand<WriteCommandOptions, WriteCommandOptionsHandler>
{
    public WriteCommand() : base("write", "Write data on specific entity path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The storage's path to which the data is to be written") { IsRequired = true };

        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: "Data (only string or base64 data is supported) should be written");

        var overWriteOption = new Option<bool?>(new[] { "-o", "--overwrite" }, 
            getDefaultValue: () => false,
            description: "Overwriting data on storage entity if the entity exists");

        var fileToUploadOption = new Option<string?>(new[] { "-f", "--file-to-upload" },
            description: "The local file path to upload");

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(dataOption);
        AddOption(overWriteOption);
        AddOption(fileToUploadOption);
        AddOption(urlOption);
    }
}