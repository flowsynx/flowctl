using System.CommandLine;
using FlowSynx.Environment;
using FlowSynx.IO.Serialization;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Health;

internal class HealthCommand : BaseCommand<HealthCommandOptions, HealthCommandOptionsHandler>
{
    public HealthCommand() : base("health", "Configuration management")
    {
        var outputOption = new Option<Output>(new[] { "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(outputOption);
    }
}

internal class HealthCommandOptions : ICommandOptions
{
    public Output Output { get; set; } = Output.Json;
}

internal class HealthCommandOptionsHandler : ICommandOptionsHandler<HealthCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;
    private readonly IVersion _version;

    public HealthCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService, ISerializer serializer,
        IVersion version)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(serializer, nameof(serializer));
        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
        _version = version;
    }

    public async Task<int> HandleAsync(HealthCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(HealthCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "health";
            var result = await _httpRequestService.GetAsync<HealthCheckResponse>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", cancellationToken);

            _outputFormatter.Write(result.HealthChecks, options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}

internal class HealthCheckResponse
{
    public string? Status { get; set; }
    public IEnumerable<IndividualHealthCheckResponse> HealthChecks { get; set; } = new List<IndividualHealthCheckResponse>();
    public TimeSpan HealthCheckDuration { get; set; }
}

internal class IndividualHealthCheckResponse
{
    public string? Status { get; set; }
    public string? Component { get; set; }
    public string? Description { get; set; }
}