using FlowCtl.Core.Services.Logger;
using FlowSynx.Client;

namespace FlowCtl.Commands.Health;

internal class HealthCommandOptionsHandler : ICommandOptionsHandler<HealthCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;

    public HealthCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, 
        IFlowSynxClient flowSynxClient)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _flowSynxClient = flowSynxClient ?? throw new ArgumentNullException(nameof(flowSynxClient));
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
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            var result = await _flowSynxClient.HealthCheck.Check(cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.Commands_Error_DuringProcessingRequest);

            var payload = result.Payload;
            _flowCtlLogger.Write(payload.HealthChecks.ToList(), options.Output);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}