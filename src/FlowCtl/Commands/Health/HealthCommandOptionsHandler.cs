using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;

namespace FlowCtl.Commands.Health;

internal class HealthCommandOptionsHandler : ICommandOptionsHandler<HealthCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;

    public HealthCommandOptionsHandler(IOutputFormatter outputFormatter, 
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
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
            _outputFormatter.Write(payload.HealthChecks.ToList(), options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}