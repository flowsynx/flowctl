using EnsureThat;
using FlowCtl.Core.Logger;
using FlowCtl.Core.Serialization;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Config;

namespace FlowCtl.Commands.Config.Add;

internal class AddConfigCommandOptionsHandler : ICommandOptionsHandler<AddConfigCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IJsonDeserializer _deserializer;

    public AddConfigCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IJsonDeserializer deserializer)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _deserializer = deserializer;
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
            var result = await _flowSynxClient.AddConfig(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _flowCtlLogger.WriteError(payload.Messages);
            else
                _flowCtlLogger.Write(payload.Data);
        }
        //catch (DeserializerException)
        //{
        //    _flowCtlLogger.WriteError(Resources.AddConfigCommandCouldNotParseSpecifications);
        //}
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }

    private AddConfigRequest AddConfigData(string? json)
    {
        return _deserializer.Deserialize<AddConfigRequest>(json);
    }
}