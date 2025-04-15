using EnsureThat;
using FlowCtl.Core.Logger;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Connectors;

namespace FlowCtl.Commands.Plugins.Details;

internal class PluginDetailsCommandOptionsHandler : ICommandOptionsHandler<PluginDetailsCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;

    public PluginDetailsCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(PluginDetailsCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(PluginDetailsCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new ConnectorDetailsRequest { Type = options.Type};
            var result = await _flowSynxClient.ConnectorDetails(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _flowCtlLogger.WriteError(payload.Messages);
            else
                _flowCtlLogger.Write(payload.Data, options.Output);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}