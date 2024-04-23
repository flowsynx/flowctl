using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadCommand : BaseCommand<ReadCommandOptions, ReadCommandOptionsHandler>
{
    public ReadCommand() : base("read", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };
        var hashOption = new Option<bool?>(new[] { "-h", "--hashing" }, getDefaultValue: () => false, "The path to get about");
        var savePathOption = new Option<string>(new[] { "-s", "--save-to" }, "The path to get about") { IsRequired = true };
        var overWriteOption = new Option<bool?>(new[] { "-o", "--overwrite" }, getDefaultValue: () => false, "The path to get about");
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(hashOption);
        AddOption(savePathOption);
        AddOption(overWriteOption);
        AddOption(urlOption);
    }
}