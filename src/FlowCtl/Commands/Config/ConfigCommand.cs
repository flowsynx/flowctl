using System.CommandLine;
using FlowCtl.Commands.Config.Add;
using FlowCtl.Commands.Config.Delete;
using FlowCtl.Commands.Config.Details;
using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Config;

internal class ConfigCommand : BaseCommand<ConfigCommandOptions, ConfigCommandOptionsHandler>
{
    public ConfigCommand() : base("config", Resources.ConfigCommandDescription)
    {
        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.CommandFieldOption);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);
        
        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new AddConfigCommand());
        AddCommand(new DeleteConfigCommand());
        AddCommand(new DetailsConfigCommand());
    }
}