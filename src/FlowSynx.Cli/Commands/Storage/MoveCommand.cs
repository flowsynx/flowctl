using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using static FlowSynx.Cli.Commands.Storage.CopyCommandOptionsHandler;

namespace FlowSynx.Cli.Commands.Storage;

internal class MoveCommand : BaseCommand<MoveCommandOptions, MoveCommandOptionsHandler>
{
    public MoveCommand() : base("move", "List of entities regarding specific path")
    {
        var sourcePathOption = new Option<string>(new[] { "--source-path" }, "The path to get about") { IsRequired = true };
        var destinationPathOption = new Option<string>(new[] { "-d", "--destination-path" }, "The path to get about") { IsRequired = true };
        var includeOption = new Option<string?>(new[] { "--include" }, "Include entities matching pattern");
        var excludeOption = new Option<string?>(new[] { "--exclude" }, "Exclude entities matching pattern");
        var minAgeOption = new Option<string?>(new[] { "--min-age" }, "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var maxAgeOption = new Option<string?>(new[] { "--max-age" }, "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var minSizeOption = new Option<string?>(new[] { "--min-size" }, "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var maxSizeOption = new Option<string?>(new[] { "--max-size" }, "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var caseSensitiveOption = new Option<bool?>(new[] { "--case-sensitive" }, getDefaultValue: () => false, "Ignore or apply case sensitive in filters");
        var recurseOption = new Option<bool?>(new[] { "--recurse" }, getDefaultValue: () => false, "Apply recursion on filtering entities in the specified path");

        AddOption(sourcePathOption);
        AddOption(destinationPathOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
    }
}

internal class MoveCommandOptions : ICommandOptions
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public string? MinSize { get; set; } = string.Empty;
    public string? MaxSize { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
}

internal class MoveCommandOptionsHandler : ICommandOptionsHandler<MoveCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public MoveCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(MoveCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(MoveCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/move";
            var request = new MoveRequest
            {
                SourcePath = options.SourcePath,
                DestinationPath = options.DestinationPath,
                Include = options.Include,
                Exclude = options.Exclude,
                MinAge = options.MinAge,
                MaxAge = options.MaxAge,
                MinSize = options.MinSize,
                MaxSize = options.MaxSize,
                CaseSensitive = options.CaseSensitive,
                Recurse = options.Recurse
            };
            var result = await _httpRequestService.PostRequestAsync<MoveRequest, Result<MoveResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
            {
                if (result?.Data is not null)
                    _outputFormatter.Write(result.Data);
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

public class MoveRequest
{
    public required string SourcePath { get; set; }
    public required string DestinationPath { get; set; }
    public string? Include { get; set; }
    public string? Exclude { get; set; }
    public string? MinAge { get; set; }
    public string? MaxAge { get; set; }
    public string? MinSize { get; set; }
    public string? MaxSize { get; set; }
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
    public bool? ClearDestinationPath { get; set; } = false;
    public bool? CreateEmptyDirectories { get; set; } = true;
}

public class MoveResponse
{

}