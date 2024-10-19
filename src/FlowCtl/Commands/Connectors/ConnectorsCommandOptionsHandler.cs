using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Connectors;

namespace FlowCtl.Commands.Connectors;

internal class ConnectorsCommandOptionsHandler : ICommandOptionsHandler<ConnectorsCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;

    public ConnectorsCommandOptionsHandler(IOutputFormatter outputFormatter,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(ConnectorsCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(ConnectorsCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new ConnectorsListRequest
            {
                Fields = options.Fields,
                Filter = options.Filter,
                CaseSensitive = options.CaseSensitive,
                Sort = options.Sort,
                Limit = options.Limit
            };
            var result = await _flowSynxClient.ConnectorsList(request, cancellationToken);

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