using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Plugins;

internal class PluginDetailsCommand : BaseCommand<DetailsPluginCommandOptions, DetailsPluginCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", "About storage")
    {
        var nameOption = new Option<Guid>(new[] { "--id" }, "The path to get about") { IsRequired = true };
        var outputFormatOption = new Option<Output>(new[] { "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(nameOption);
        AddOption(outputFormatOption);
    }
}

internal class DetailsPluginCommandOptions : ICommandOptions
{
    public Guid Id { get; set; } = Guid.Empty;
    public Output Output { get; set; } = Output.Json;
}

internal class DetailsPluginCommandOptionsHandler : ICommandOptionsHandler<DetailsPluginCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public DetailsPluginCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(DetailsPluginCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(DetailsPluginCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var relativeUrl = $"plugins/details/{options.Id}";
            var result = await _httpRequestService.GetRequestAsync<Result<PluginDetailsResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
                _outputFormatter.Write(result?.Data, options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}

public class PluginDetailsResponse
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
}