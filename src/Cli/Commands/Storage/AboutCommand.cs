using System.CommandLine;
using Cli.Extensions;
using Cli.Models;
using Cli.Serialization;
using Cli.Services;
using Spectre.Console;

namespace Cli.Commands.Storage;

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
    private readonly IFlowSyncDefaultEndpoint _defaultEndpoint;
    private readonly IHttpHandler _httpHandler;
    private readonly ISerializer _serializer;

    public AboutCommandOptionsHandler(IAnsiConsole console, IFlowSyncDefaultEndpoint defaultEndpoint, IHttpHandler httpHandler, 
        ISerializer serializer)
    {
        _console = console;
        _defaultEndpoint = defaultEndpoint;
        _httpHandler = httpHandler;
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
            var result = await _httpHandler.PostRequest<AboutCommandOptions, Result<object>>($"{_defaultEndpoint.GetDefaultHttpEndpoint()}/{url}", options, cancellationToken);
            
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