using System.CommandLine;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", Resources.VersionCommandDescription)
    {
        var typeOption = new Option<bool>(new[] { "-f", "--full" }, 
            getDefaultValue: () => false,
            description: Resources.VersionCommandFullOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json, 
            description: Resources.CommandOutputOption);

        AddOption(typeOption);
        AddOption(outputOption);
    }
}