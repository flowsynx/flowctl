using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Plugins;

namespace FlowCtl.Commands.Plugins;

internal class PluginsCommandOptionsHandler : ICommandOptionsHandler<PluginsCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;

    public PluginsCommandOptionsHandler(IOutputFormatter outputFormatter,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(PluginsCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(PluginsCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new PluginsListRequest
            {
                Include = options.Include,
                Exclude = options.Exclude,
                CaseSensitive = options.CaseSensitive,
                MaxResults = options.MaxResults,
                Sorting = options.Sorting
            };
            var result = await _flowSynxClient.PluginsList(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _outputFormatter.WriteError(payload.Messages);
            else
                _outputFormatter.Write(payload.Data, options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}