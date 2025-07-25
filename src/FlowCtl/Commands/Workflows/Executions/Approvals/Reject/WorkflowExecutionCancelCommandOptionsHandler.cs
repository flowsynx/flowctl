﻿using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Workflows;

namespace FlowCtl.Commands.Workflows.Executions.Approvals.Reject;

internal class WorkflowExecutionRejectPendingTaskCommandOptionsHandler : ICommandOptionsHandler<WorkflowExecutionRejectPendingTaskCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public WorkflowExecutionRejectPendingTaskCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(WorkflowExecutionRejectPendingTaskCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(WorkflowExecutionRejectPendingTaskCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            if (!Guid.TryParse(options.WorkflowId, out Guid workflowId))
                throw new FormatException("Invalid workflow id format. Expected a valid GUID.");

            if (!Guid.TryParse(options.ExecutionId, out Guid executionId))
                throw new FormatException("Invalid execution id format. Expected a valid GUID.");

            if (!Guid.TryParse(options.ApprovalId, out Guid approvalId))
                throw new FormatException("Invalid approval id format. Expected a valid GUID.");

            var request = new RejectWorkflowRequest
            {
                WorkflowId = workflowId, 
                WorkflowExecutionId = executionId,
                WorkflowExecutionApprovalId = approvalId
            };
            var result = await _flowSynxClient.Workflows.RejectExecutionsAsync(request, cancellationToken);

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