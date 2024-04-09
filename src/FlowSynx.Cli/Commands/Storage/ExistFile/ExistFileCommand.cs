using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.ExistFile;

internal class ExistFileCommand : BaseCommand<ExistFileCommandOptions, ExistFileCommandOptionsHandler>
{
    public ExistFileCommand() : base("exist", "Check the entity exist on specific path")
    {
        var pathOption = new Option<string>("--path", "The path to get about") { IsRequired = true };

        AddOption(pathOption);
    }
}