using EnsureThat;
using FlowCtl.Core.Logger;
using FlowCtl.Core.Serialization;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Config;

namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommandOptionsHandler : ICommandOptionsHandler<DeleteConfigCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IJsonDeserializer _deserializer;

    public DeleteConfigCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IJsonDeserializer deserializer)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _deserializer = deserializer;
    }

    public async Task<int> HandleAsync(DeleteConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(DeleteConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
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

            var request = DeleteConfigData(jsonData);
            var result = await _flowSynxClient.DeleteConfig(request, cancellationToken);

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

    private DeleteConfigRequest DeleteConfigData(string? json)
    {
        return _deserializer.Deserialize<DeleteConfigRequest>(json);
    }
}