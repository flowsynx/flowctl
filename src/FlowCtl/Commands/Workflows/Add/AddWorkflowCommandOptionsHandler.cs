using FlowCtl.Core.Authentication;
using FlowCtl.Core.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Workflows;

namespace FlowCtl.Commands.Workflows.Add;

internal class AddWorkflowCommandOptionsHandler : ICommandOptionsHandler<AddWorkflowCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public AddWorkflowCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(AddWorkflowCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(AddWorkflowCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

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

            var request = new AddWorkflowRequest { Definition = definitionJsonData };
            var result = await _flowSynxClient.AddWorkflow(request, cancellationToken);

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