using EnsureThat;
using FlowCtl.Core.Logger;
using FlowCtl.Core.Serialization;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Connectors;

namespace FlowCtl.Commands.Plugins;

internal class PluginsCommandOptionsHandler : ICommandOptionsHandler<PluginsCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IJsonDeserializer _deserializer;

    public PluginsCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IJsonDeserializer deserializer)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _deserializer = deserializer;
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

            var request = new ConnectorsListRequest();
            var result = await _flowSynxClient.ConnectorsList(request, cancellationToken);

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