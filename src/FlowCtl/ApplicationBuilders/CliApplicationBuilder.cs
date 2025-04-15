﻿using EnsureThat;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace FlowCtl.ApplicationBuilders;

public class CliApplicationBuilder : ICliApplicationBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RootCommand _rootCommand;

    public CliApplicationBuilder(IServiceProvider serviceProvider, RootCommand rootCommand)
    {
        EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
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
            .UseHelp()
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .UseExceptionHandler()
            .CancelOnProcessTermination();

        return await commandLineBuilder.Build().InvokeAsync(args);
    }
}