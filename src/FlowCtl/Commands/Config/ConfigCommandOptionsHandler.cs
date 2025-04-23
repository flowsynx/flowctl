using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;

namespace FlowCtl.Commands.Config;

internal class ConfigCommandOptionsHandler : ICommandOptionsHandler<ConfigCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public ConfigCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(authenticationManager);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(ConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(ConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var result = await _flowSynxClient.PluginConfigList(cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.Commands_Error_DuringProcessingRequest);

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