using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Config;
using FlowSynx.Client.Responses.Config;

namespace FlowSynx.Cli.Commands.Config;

internal class ConfigCommandOptionsHandler : ICommandOptionsHandler<ConfigCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public ConfigCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(ConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(ConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Url))
                _flowSynxClient.ChangeConnection(options.Url);

            var request = new ConfigListRequest { Type = options.Type };
            var result = await _flowSynxClient.ConfigList(request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
                _outputFormatter.Write(result?.Data, options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}