using System.CommandLine;
using FlowCtl.Commands.Config.Add;
using FlowCtl.Commands.Config.Delete;
using FlowCtl.Commands.Config.Details;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Config;

internal class ConfigCommand : BaseCommand<ConfigCommandOptions, ConfigCommandOptionsHandler>
{
    public ConfigCommand() : base("config", Resources.Commands_Config_Description)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);
        
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new AddConfigCommand());
        AddCommand(new DeleteConfigCommand());
        AddCommand(new DetailsConfigCommand());
    }
}