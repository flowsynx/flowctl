using FlowCtl.Core.Authentication;
using FlowCtl.Core.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Plugins;

namespace FlowCtl.Commands.Plugins.Add;

internal class AddPluginCommandOptionsHandler : ICommandOptionsHandler<AddPluginCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public AddPluginCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(AddPluginCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(AddPluginCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new AddPluginRequest { Type = options.Type, Version = options.Version };
            var result = await _flowSynxClient.AddPlugin(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _flowCtlLogger.WriteError(payload.Messages);
            else
                _flowCtlLogger.Write(payload.Data);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}