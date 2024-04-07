using EnsureThat;
using FlowSynx.Abstractions;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO.Exceptions;
using FlowSynx.IO.Serialization;
using FlowSynx.Net;

namespace FlowSynx.Cli.Commands.Config.Add;

internal class AddConfigCommandOptionsHandler : ICommandOptionsHandler<AddConfigCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;
    private readonly IDeserializer _deserializer;

    public AddConfigCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService, IDeserializer deserializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
        _deserializer = deserializer;
    }

    public async Task<int> HandleAsync(AddConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(AddConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "config/add";

            var specification = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(options.Spec))
            {
                specification = _deserializer.Deserialize<Dictionary<string, string?>>(options.Spec);
            }

            var request = new AddConfigRequest { Name = options.Name, Type = options.Type, Specifications = specification };
            var result = await _httpRequestService.PostRequestAsync<AddConfigRequest, Result<AddConfigResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
                _outputFormatter.Write(result?.Data);
        }
        catch (DeserializerException)
        {
            _outputFormatter.WriteError("Could not parse the entered specifications. Please check it and try again!");
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}