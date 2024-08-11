using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Config;

namespace FlowCtl.Commands.Config;

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
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new ConfigListRequest
            {
                Include = options.Include, 
                Exclude = options.Exclude,
                CaseSensitive = options.CaseSensitive,
                MinAge = options.MinAge,
                MaxAge = options.MaxAge,
                MaxResults = options.MaxResults,
                Sorting = options.Sorting
            };
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