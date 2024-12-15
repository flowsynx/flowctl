using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Config;
using FlowSynx.Client.Requests.Connectors;
using FlowSynx.IO.Serialization;

namespace FlowCtl.Commands.Connectors;

internal class ConnectorsCommandOptionsHandler : ICommandOptionsHandler<ConnectorsCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IDeserializer _deserializer;

    public ConnectorsCommandOptionsHandler(IOutputFormatter outputFormatter,
        IFlowSynxClient flowSynxClient, IDeserializer deserializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
        _flowSynxClient = flowSynxClient;
        _deserializer = deserializer;
    }

    public async Task<int> HandleAsync(ConnectorsCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(ConnectorsCommandOptions options, CancellationToken cancellationToken)
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

            var request = GetConnectorsListData(jsonData);
            var result = await _flowSynxClient.ConnectorsList(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _outputFormatter.WriteError(payload.Messages);
            else
                _outputFormatter.Write(payload.Data, options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private ConnectorsListRequest GetConnectorsListData(string? json)
    {
        var result = new ConnectorsListRequest();

        if (!string.IsNullOrEmpty(json))
            return _deserializer.Deserialize<ConnectorsListRequest>(json);

        return result;
    }
}