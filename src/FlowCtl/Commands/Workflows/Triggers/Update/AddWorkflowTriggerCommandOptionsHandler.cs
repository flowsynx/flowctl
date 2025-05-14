using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Workflows;

namespace FlowCtl.Commands.Workflows.Triggers.Update;

internal class UpdateWorkflowTriggerCommandOptionsHandler : ICommandOptionsHandler<UpdateWorkflowTriggerCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IJsonDeserializer _deserializer;
    private readonly IAuthenticationManager _authenticationManager;

    public UpdateWorkflowTriggerCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IJsonDeserializer deserializer,
        IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _deserializer = deserializer;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(UpdateWorkflowTriggerCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(UpdateWorkflowTriggerCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            string? jsonData;
            if (!string.IsNullOrEmpty(options.DataFile))
            {
                if (!File.Exists(options.DataFile))
                    throw new Exception(string.Format(Resources.Command_Workflow_Triggers_UpdateCommand_FileNotExist, options.DataFile));

                jsonData = await File.ReadAllTextAsync(options.DataFile, cancellationToken);
            }
            else
            {
                jsonData = options.Data;
            }

            if (!Guid.TryParse(options.WorkflowId, out Guid workflowId))
                throw new FormatException("Invalid workflow id format. Expected a valid GUID.");

            if (!Guid.TryParse(options.TriggerId, out Guid triggerId))
                throw new FormatException("Invalid trigger id format. Expected a valid GUID.");

            var request = GetUpdateTriggerData(jsonData);
            request.WorkflowId = workflowId;
            request.TriggerId = triggerId;
            var result = await _flowSynxClient.Workflows.UpdateTriggerAsync(request, cancellationToken);

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

    private UpdateWorkflowTriggerRequest GetUpdateTriggerData(string? json)
    {
        return _deserializer.Deserialize<UpdateWorkflowTriggerRequest>(json);
    }
}