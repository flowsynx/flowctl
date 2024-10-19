using System.CommandLine;
using FlowCtl.Commands.Connectors.Details;

namespace FlowCtl.Commands.Connectors;

internal class ConnectorsCommand : BaseCommand<ConnectorsCommandOptions, ConnectorsCommandOptionsHandler>
{
    public ConnectorsCommand() : base("connectors", Resources.ConnectorsCommandDescription)
    {
        var fieldsOption = new Option<string[]?>(new[] { "-f", "--fields" },
            description: Resources.CommandFieldOption);

        var filterOption = new Option<string?>(new[] { "+f", "--filter" },
            description: Resources.CommandFilterOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-c", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var sortOption = new Option<string?>(new[] { "-s", "--sort" },
            description: Resources.CommandSortOption);

        var limitOption = new Option<string?>(new[] { "-l", "--limit" },
            description: Resources.CommandLimitOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" },
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(fieldsOption);
        AddOption(filterOption);
        AddOption(caseSensitiveOption);
        AddOption(sortOption);
        AddOption(limitOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new ConnectorDetailsCommand());
    }
}