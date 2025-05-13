using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Workflows;

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
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            string? definitionJsonData;
            if (!string.IsNullOrEmpty(options.DefinitionFile))
            {
                if (!File.Exists(options.DefinitionFile))
                    throw new Exception(string.Format(Resources.Command_Workflow_AddCommand_FileNotExist, options.DefinitionFile));

                definitionJsonData = await File.ReadAllTextAsync(options.DefinitionFile, cancellationToken);
            }
            else
            {
                definitionJsonData = options.Definition;
            }

            var request = new AddWorkflowRequest { Definition = definitionJsonData };
            var result = await _flowSynxClient.Workflows.AddAsync(request, cancellationToken);

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