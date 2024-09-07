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

            var specification = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(options.Spec))
                specification = _deserializer.Deserialize<Dictionary<string, string?>>(options.Spec);
            
            var request = new AddConfigRequest { Name = options.Name, Type = options.Type, Specifications = specification };
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
}