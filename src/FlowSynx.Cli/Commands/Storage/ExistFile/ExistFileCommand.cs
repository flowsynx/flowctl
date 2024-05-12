using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.ExistFile;

internal class ExistFileCommand : BaseCommand<ExistFileCommandOptions, ExistFileCommandOptionsHandler>
{
    public ExistFileCommand() : base("exist", "Check the entity exist on specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, 
            description: "The entity's path that going to be checked for existence") { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(addressOption);
    }
}