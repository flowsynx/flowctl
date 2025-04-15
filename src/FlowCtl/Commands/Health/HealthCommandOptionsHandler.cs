using EnsureThat;
using FlowCtl.Core.Logger;
using FlowSynx.Client;

namespace FlowCtl.Commands.Health;

internal class HealthCommandOptionsHandler : ICommandOptionsHandler<HealthCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;

    public HealthCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, 
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(HealthCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(HealthCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var result = await _flowSynxClient.Health(cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            _flowCtlLogger.Write(payload.HealthChecks.ToList(), options.Output);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}