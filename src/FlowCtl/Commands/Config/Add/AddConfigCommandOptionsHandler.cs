using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Config;
using FlowSynx.IO.Exceptions;
using FlowSynx.IO.Serialization;

namespace FlowCtl.Commands.Config.Add;

internal class AddConfigCommandOptionsHandler : ICommandOptionsHandler<AddConfigCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IDeserializer _deserializer;

    public AddConfigCommandOptionsHandler(IOutputFormatter outputFormatter,
        IFlowSynxClient flowSynxClient, IDeserializer deserializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

        _outputFormatter = outputFormatter;
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
                _outputFormatter.WriteError(payload.Messages);
            else
                _outputFormatter.Write(payload.Data);
        }
        catch (DeserializerException)
        {
            _outputFormatter.WriteError(Resources.AddConfigCommandCouldNotParseSpecifications);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private AddConfigRequest AddConfigData(string? json)
    {
        return _deserializer.Deserialize<AddConfigRequest>(json);
    }
}