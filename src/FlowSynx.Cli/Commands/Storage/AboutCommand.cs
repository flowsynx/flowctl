using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Storage;

internal class AboutCommand : BaseCommand<AboutCommandOptions, AboutCommandOptionsHandler>
{
    public AboutCommand() : base("about", "About storage")
    {
        var pathOption = new Option<string>(new[] { "--path" }, "The path to get about") { IsRequired = true };
        var fullOption = new Option<bool?>(new[] { "--full" }, "Should apply format for byte size");
        var outputFormatOption = new Option<Output>(new[] { "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(pathOption);
        AddOption(fullOption);
        AddOption(outputFormatOption);
    }
}

internal class AboutCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public bool? Full { get; set; } = false;
    public Output Output { get; set; } = Output.Json;
}

internal class AboutCommandOptionsHandler : ICommandOptionsHandler<AboutCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public AboutCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(AboutCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(AboutCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/about";
            var request = new AboutRequest()
            {
                Path = options.Path, 
                Full = options.Full
            };

            var result = await _httpRequestService.PostRequestAsync<AboutRequest, Result<AboutResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
            {
                if (result?.Data is not null)
                    _outputFormatter.Write(result.Data, options.Output);
                else
                    _outputFormatter.Write(result?.Messages);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}

public class AboutRequest
{
    public required string Path { get; set; }
    public bool? Full { get; set; } = false;
}

public class AboutResponse
{
    public string? Total { get; set; }
    public string? Free { get; set; }
    public string? Used { get; set; }
}
