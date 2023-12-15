using EnsureThat;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Cli.Extensions;
using Cli.Resources;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Cli.ApplicationBuilders;

public class CliApplicationBuilder : ICliApplicationBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RootCommand _rootCommand;

    public CliApplicationBuilder(IServiceProvider serviceProvider, RootCommand rootCommand)
    {
        EnsureArg.IsNotNull(rootCommand, nameof(rootCommand));
        _serviceProvider = serviceProvider;
        _rootCommand = rootCommand;
    }

    public async Task<int> RunAsync(string[] args)
    {
        var commands = _serviceProvider?.GetServices<Command>();
        commands?.ToList().ForEach(cmd => _rootCommand.AddCommand(cmd));

        var commandLineBuilder = new CommandLineBuilder(_rootCommand)
            .AddMiddleware(async (context, next) =>
            {
                context.BindingContext.AddService<IServiceProvider>(_ => _serviceProvider!);
                await next(context);
            })
            .UseDefaults();

        var console = _serviceProvider?.GetService<IAnsiConsole>() ?? AnsiConsole.Console;

        if (!args.Any())
            console.WriteText(FlowSyncLogo());

        return await commandLineBuilder.Build().InvokeAsync(args);

    }

    private string FlowSyncLogo() => CliResource.logo;
}