using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Health;

internal class HealthCommand : BaseCommand<HealthCommandOptions, HealthCommandOptionsHandler>
{
    public HealthCommand() : base("health", Resources.Commands_Health_Description)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" }, 
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(addressOption);
        AddOption(outputOption);
    }
}