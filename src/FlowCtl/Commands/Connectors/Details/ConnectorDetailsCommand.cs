using System.CommandLine;

namespace FlowCtl.Commands.Connectors.Details;

internal class ConnectorDetailsCommand : BaseCommand<ConnectorDetailsCommandOptions, ConnectorDetailsCommandOptionsHandler>
{
    public ConnectorDetailsCommand() : base("details", Resources.ConnectorDetailsCommandDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(typeOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}