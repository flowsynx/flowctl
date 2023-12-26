using System.CommandLine;
using FlowSynx.Cli.Extensions;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.IO.Serialization;
using FlowSynx.Net;
using Spectre.Console;
using EnsureThat;

namespace FlowSynx.Cli.Commands.Storage;

internal class AboutCommand : BaseCommand<AboutCommandOptions, AboutCommandOptionsHandler>
{
    public AboutCommand() : base("about", "About storage")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };
        var fullOption = new Option<bool?>(new[] { "-f", "--full" }, "Should apply format for byte size");

        AddOption(pathOption); 
        AddOption(fullOption);
    }
}

internal class AboutCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public bool? Full { get; set; } = false;
}

internal class AboutCommandOptionsHandler : ICommandOptionsHandler<AboutCommandOptions>
{
    private readonly IAnsiConsole _console;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;
    private readonly ISerializer _serializer;

    public AboutCommandOptionsHandler(IAnsiConsole console, IEndpoint endpoint, IHttpRequestService httpRequestService, 
        ISerializer serializer)
    {
        EnsureArg.IsNotNull(console, nameof(console));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(serializer, nameof(serializer));
        _console = console;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
        _serializer = serializer;
    }

    public async Task<int> HandleAsync(AboutCommandOptions options, CancellationToken cancellationToken)
    {
        await _console.DisplayLineSpinnerAsync(async () => await CallAboutApi(options, cancellationToken));
        return 0;
    }

    private async Task CallAboutApi(AboutCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string url = "storage/about";
            var result = await _httpRequestService.PostAsync<AboutCommandOptions, Result<object>>($"{_endpoint.GetDefaultHttpEndpoint()}/{url}", options, cancellationToken);
            
            if (!result.Succeeded)
                _console.WriteError(_serializer.Serialize(result.Messages));
            else
                _console.WriteText(_serializer.Serialize(result.Data));
        }
        catch (Exception ex)
        {
            _console.WriteError(ex.Message);
        }
    }
}