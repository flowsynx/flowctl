using System.CommandLine;

namespace FlowCtl.Commands.Config.Add;

internal class AddConfigCommand : BaseCommand<AddConfigCommandOptions, AddConfigCommandOptionsHandler>
{
    public AddConfigCommand() : base("add", Resources.Commands_AddConfig_Description)
    {
        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.Commands_AddConfig_Data);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.Commands_AddConfig_DataFile);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
    }
}