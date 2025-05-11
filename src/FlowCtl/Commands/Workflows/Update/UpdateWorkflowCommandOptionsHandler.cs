using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Workflows;

namespace FlowCtl.Commands.Workflows.Update;

internal class UpdateWorkflowCommandOptionsHandler : ICommandOptionsHandler<UpdateWorkflowCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public UpdateWorkflowCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(UpdateWorkflowCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(UpdateWorkflowCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            string? definitionJsonData;
            if (!string.IsNullOrEmpty(options.DefinitionFile))
            {
                if (!File.Exists(options.DefinitionFile))
                    throw new Exception($"Entered definition file '{options.DefinitionFile}' is not exist.");

                definitionJsonData = await File.ReadAllTextAsync(options.DefinitionFile, cancellationToken);
            }
            else
            {
                definitionJsonData = options.Definition;
            }

            var request = new UpdateWorkflowRequest { Id = Guid.Parse(options.Id) , Definition = definitionJsonData };
            var result = await _flowSynxClient.Workflows.UpdateAsync(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.Commands_Error_DuringProcessingRequest);

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