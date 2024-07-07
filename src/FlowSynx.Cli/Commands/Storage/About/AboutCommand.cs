using System.CommandLine;

namespace FlowCtl.Commands.Storage.About;

internal class AboutCommand : BaseCommand<AboutCommandOptions, AboutCommandOptionsHandler>
{
    public AboutCommand() : base("about", Resources.StorageAboutCommandDescription)
    {
        var pathOption = new Option<string?>(new[] { "-p", "--path" },
            description: Resources.CommandPathOption);

        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, 
            getDefaultValue: () => false,
            description: Resources.CommandFullOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(pathOption);
        AddOption(fullOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}