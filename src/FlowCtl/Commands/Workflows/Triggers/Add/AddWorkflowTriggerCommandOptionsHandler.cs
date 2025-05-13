using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Workflows;

namespace FlowCtl.Commands.Workflows.Triggers.Add;

internal class AddWorkflowTriggerCommandOptionsHandler : ICommandOptionsHandler<AddWorkflowTriggerCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IJsonDeserializer _deserializer;
    private readonly IAuthenticationManager _authenticationManager;

    public AddWorkflowTriggerCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
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

    public async Task<int> HandleAsync(AddWorkflowTriggerCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(AddWorkflowTriggerCommandOptions options, CancellationToken cancellationToken)
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
                    throw new Exception(string.Format(Resources.Command_Workflow_Triggers_AddCommand_FileNotExist, options.DataFile));

                jsonData = await File.ReadAllTextAsync(options.DataFile, cancellationToken);
            }
            else
            {
                jsonData = options.Data;
            }

            var request = AddTriggerData(jsonData);
            request.WorkflowId = options.WorkflowId;
            var result = await _flowSynxClient.Workflows.AddTriggerAsync(request, cancellationToken);

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

    private AddWorkflowTriggerRequest AddTriggerData(string? json)
    {
        return _deserializer.Deserialize<AddWorkflowTriggerRequest>(json);
    }
}