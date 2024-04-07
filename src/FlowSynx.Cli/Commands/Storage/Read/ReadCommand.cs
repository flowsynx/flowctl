using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadCommand : BaseCommand<ReadCommandOptions, ReadCommandOptionsHandler>
{
    public ReadCommand() : base("read", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>("--path", "The path to get about") { IsRequired = true };
        var savePathOption = new Option<string>("--save-to", "The path to get about") { IsRequired = true };
        var overWriteOption = new Option<bool?>("--overwrite", getDefaultValue: () => false, "The path to get about");

        AddOption(pathOption);
        AddOption(savePathOption);
        AddOption(overWriteOption);
    }
}