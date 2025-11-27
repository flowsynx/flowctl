using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Workflows;
using FlowSynx.Client.Messages.Responses.Workflows;

namespace FlowCtl.Commands.Workflows.Executions.Execute;

internal class ExecuteWorkflowCommandOptionsHandler : ICommandOptionsHandler<ExecuteWorkflowCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public ExecuteWorkflowCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(ExecuteWorkflowCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(ExecuteWorkflowCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            Guid workflowId;

            if (!string.IsNullOrWhiteSpace(options.WorkflowId))
            {
                // Execute existing workflow by id
                if (!Guid.TryParse(options.WorkflowId, out workflowId))
                    throw new FormatException("Invalid workflow id format. Expected a valid GUID.");
            }
            else
            {
                // Need to add workflow first using provided definition
                if (string.IsNullOrWhiteSpace(options.Definition) && string.IsNullOrWhiteSpace(options.DefinitionFile))
                    throw new Exception("Either a workflow id (-w) or a definition (-d / -f) must be provided.");

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

                if (string.IsNullOrWhiteSpace(definitionJsonData))
                    throw new Exception("Workflow definition content is empty.");

                var addRequest = new AddWorkflowRequest { Definition = definitionJsonData };
                var addResult = await _flowSynxClient.Workflows.AddAsync(addRequest, cancellationToken);

                if (addResult.StatusCode != 200)
                    throw new Exception(Resources.Commands_Error_DuringProcessingRequest);

                var addPayload = addResult.Payload;
                if (addPayload is { Succeeded: false })
                {
                    _flowCtlLogger.WriteError(addPayload.Messages);
                    return;
                }

                workflowId = ExtractWorkflowId(addPayload.Data) ?? throw new Exception("Unable to determine newly added workflow id from response.");
            }

            var execRequest = new ExecuteWorkflowRequest { WorkflowId = workflowId };
            var execResult = await _flowSynxClient.Workflows.ExecuteAsync(execRequest, cancellationToken);

            if (execResult.StatusCode != 200)
                throw new Exception(Resources.Commands_Error_DuringProcessingRequest);

            var execPayload = execResult.Payload;
            if (execPayload is { Succeeded: false })
                _flowCtlLogger.WriteError(execPayload.Messages);
            else
                _flowCtlLogger.Write(execPayload.Data);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }

    private Guid? ExtractWorkflowId(AddWorkflowResponse? data)
    {
        if (data is null) return null;
        return data.Id;
    }
}