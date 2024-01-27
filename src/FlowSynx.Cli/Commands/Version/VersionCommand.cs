using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommand : BaseCommand<VersionCommandOptions, VersionCommandOptionsHandler>
{
    public VersionCommand() : base("version", "Display the FlowSynx system and Cli version")
    {
        var typeOption = new Option<bool>(new[] { "--full" }, getDefaultValue: () => false, "Display full details about the running FlowSynx system");
        var outputOption = new Option<Output>(new[] { "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(typeOption);
        AddOption(outputOption);
    }
}

internal class VersionCommandOptions : ICommandOptions
{
    public bool? Full { get; set; } = false;
    public Output Output { get; set; } = Output.Json;
}

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;
    private readonly IVersion _version;

    public VersionCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService, IVersion version)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(version, nameof(version));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
        _version = version;
    }

    public async Task<int> HandleAsync(VersionCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(VersionCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (options.Full is null or false)
            {
                var version = new
                {
                    Cli = _version.Version
                };
                _outputFormatter.Write(version, options.Output);
                return;
            }

            const string relativeUrl = "version";
            var result = await _httpRequestService.GetRequestAsync<Result<VersionResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
            {
                if (result?.Data != null)
                {
                    result.Data.Cli = _version.Version;
                    _outputFormatter.Write(result.Data, options.Output);
                }
            }
        }
        catch (Exception)
        {
            if (options.Full is true)
            {
                var version = new
                {
                    Cli = _version.Version,
                    FlowSynx = "FlowSynx system is not running"
                };
                _outputFormatter.Write(version, options.Output);
            }
        }
    }
}

public class VersionResponse
{
    public string? Cli { get; set; }
    public required string FlowSynx { get; set; }
    public string? OSVersion { get; set; } = string.Empty;
    public string? OSArchitecture { get; set; } = string.Empty;
    public string? OSType { get; set; } = string.Empty;
}