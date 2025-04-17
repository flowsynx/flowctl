﻿using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Requests.PluginConfig;

namespace FlowCtl.Commands.Config.Add;

internal class AddConfigCommandOptionsHandler : ICommandOptionsHandler<AddConfigCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IJsonDeserializer _deserializer;
    private readonly IAuthenticationManager _authenticationManager;

    public AddConfigCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IJsonDeserializer deserializer,
        IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(deserializer);
        ArgumentNullException.ThrowIfNull(authenticationManager);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _deserializer = deserializer;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(AddConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(AddConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            string? jsonData;
            if (!string.IsNullOrEmpty(options.DataFile))
            {
                if (!File.Exists(options.DataFile))
                    throw new Exception($"Entered data file '{options.DataFile}' is not exist.");

                jsonData = await File.ReadAllTextAsync(options.DataFile, cancellationToken);
            }
            else
            {
                jsonData = options.Data;
            }

            var request = AddConfigData(jsonData);
            var result = await _flowSynxClient.AddPluginConfig(request, cancellationToken);

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

    private AddPluginConfigRequest AddConfigData(string? json)
    {
        return _deserializer.Deserialize<AddPluginConfigRequest>(json);
    }
}