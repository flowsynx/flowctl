using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadCommand : BaseCommand<ReadCommandOptions, ReadCommandOptionsHandler>
{
    public ReadCommand() : base("read", "Read and receive data stream about specific entity")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: "The entity path to be read") { IsRequired = true };

        var hashOption = new Option<bool?>(new[] { "+h", "--hashing" }, 
            getDefaultValue: () => false,
            description: "Display hashing content in response data");

        var savePathOption = new Option<string>(new[] { "-s", "--save-to" },
            description: "The path where the streaming data are going to be saved") { IsRequired = true };

        var overWriteOption = new Option<bool?>(new[] { "-o", "--overwrite" }, 
            getDefaultValue: () => false,
            description: "Overwriting the streaming data if the file (save-to) exists");

        var urlOption = new Option<string?>(new[] { "-u", "--url" },
            description: "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(hashOption);
        AddOption(savePathOption);
        AddOption(overWriteOption);
        AddOption(urlOption);
    }
}