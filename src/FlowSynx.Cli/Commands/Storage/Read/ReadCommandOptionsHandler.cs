using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.Net;

namespace FlowSynx.Cli.Commands.Storage.Read;

internal class ReadCommandOptionsHandler : ICommandOptionsHandler<ReadCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public ReadCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
    }

    public async Task<int> HandleAsync(ReadCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(ReadCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/read";
            var request = new ReadRequest { Path = options.Path };
            var result = await _httpRequestService.PostRequestAsync($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            var filePath = options.SaveTo;
            if (Directory.Exists(filePath))
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(options.Path)}";
                filePath = Path.Combine(options.SaveTo, fileName);
            }

            if (!File.Exists(filePath) || File.Exists(filePath) && options.Overwrite is true)
            {
                await StreamHelper.WriteStream(filePath, result, cancellationToken);
            }
            else
            {
                throw new Exception($"File '{filePath}' is already exist!");
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}