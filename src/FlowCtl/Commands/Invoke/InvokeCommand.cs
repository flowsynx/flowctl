using System.CommandLine;

namespace FlowCtl.Commands.Invoke;

internal class InvokeCommand : BaseCommand<InvokeCommandOptions, InvokeCommandOptionsHandler>
{
    public InvokeCommand() : base("invoke", Resources.InvokeCommandDescription)
    {
        var methodOption = new Option<string?>(new[] { "-m", "--method" },
            description: Resources.InvokeCommandMethodOption) { IsRequired = true };

        var verbOption = new Option<Verb>(new[] { "-v", "--verb" },
            getDefaultValue: () => Verb.Post,
            description: Resources.InvokeCommandVerbOption);

        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.InvokeCommandDataOption);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(methodOption);
        AddOption(verbOption);
        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}