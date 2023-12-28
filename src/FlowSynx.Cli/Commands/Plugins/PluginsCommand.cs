using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.IO.Serialization;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", "Configuration management")
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" }, "The path to get about");
        var outputOption = new Option<Output>(new[] { "-o", "--output" }, getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(typeOption);
        AddOption(outputOption);

        AddCommand(new DetailsPluginCommand());
    }
}

internal class PluginsCommandOptions : ICommandOptions
{
    public string Type { get; set; } = string.Empty;
    public Output Output { get; set; } = Output.Json;
}

internal class PluginsCommandOptionsHandler : ICommandOptionsHandler<PluginsCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public PluginsCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        IEndpoint endpoint, IHttpRequestService httpRequestService, ISerializer serializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(serializer, nameof(serializer));
        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
    }

    public async Task<int> HandleAsync(PluginsCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(PluginsCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var relativeUrl = "plugins";
            if (!string.IsNullOrEmpty(options.Type))
                relativeUrl = $"plugins/{options.Type}";

            var result = await _httpRequestService.GetAsync<Result<object?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", cancellationToken);

            if (!result.Succeeded)
                _outputFormatter.WriteError(result.Messages);
            else
            {
                if (result.Data != null)
                    _outputFormatter.Write(result.Data, options.Output);
                else
                    _outputFormatter.Write(result.Messages);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}