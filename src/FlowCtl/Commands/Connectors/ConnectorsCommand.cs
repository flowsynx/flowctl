using System.CommandLine;
using FlowCtl.Commands.Connectors.Details;

namespace FlowCtl.Commands.Connectors;

internal class ConnectorsCommand : BaseCommand<ConnectorsCommandOptions, ConnectorsCommandOptionsHandler>
{
    public ConnectorsCommand() : base("connectors", Resources.ConnectorsCommandDescription)
    {
        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.InvokeCommandDataOption);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" },
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new ConnectorDetailsCommand());
    }
}