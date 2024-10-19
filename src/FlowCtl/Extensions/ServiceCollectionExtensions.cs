using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using FlowCtl.Commands;
using FlowCtl.ApplicationBuilders;
using FlowCtl.Commands.Config;
using FlowSynx.Environment;
using FlowSynx.IO;
using FlowCtl.Commands.Connectors;
using FlowCtl.Commands.Version;
using FlowCtl.Commands.Health;
using FlowCtl.Commands.Update;
using FlowCtl.Commands.Init;
using FlowCtl.Commands.Run;
using FlowCtl.Commands.Stop;
using FlowCtl.Commands.Uninstall;
using FlowSynx.Client;
using FlowCtl.Commands.Dashboard;
using FlowCtl.Commands.Invoke;
using FlowCtl.Commands.Logs;
using FlowCtl.Services.Abstracts;
using FlowCtl.Services.Concretes;
using FlowSynx.Logging.Extensions;

namespace FlowCtl.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocation(this IServiceCollection services)
    {
        services.AddTransient<ILocation, Location>();
        return services;
    }
    
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<RootCommand, Root>()
                .AddTransient<Command, ConfigCommand>()
                .AddTransient<Command, ConnectorsCommand>()
                .AddTransient<Command, DashboardCommand>()
                .AddTransient<Command, HealthCommand>()
                .AddTransient<Command, InitCommand>()
                .AddTransient<Command, LogsCommand>()
                .AddTransient<Command, RunCommand>()
                .AddTransient<Command, InvokeCommand>()
                .AddTransient<Command, StopCommand>()
                .AddTransient<Command, UninstallCommand>()
                .AddTransient<Command, UpdateCommand>()
                .AddTransient<Command, VersionCommand>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddOperatingSystemInfo()
                .AddCompressions()
                .AddProcessHandler()
                .AddEndpoint()
                .AddSerialization()
                .AddSingleton(AnsiConsole.Console)
                .AddTransient<ICliApplicationBuilder, CliApplicationBuilder>()
                .AddScoped<IFlowSynxClient, FlowSynxClient>()
                .AddSingleton(new FlowSynxClientConnection());

        return services;
    }

    public static IServiceCollection AddLoggingService(this IServiceCollection services)
    {
        services.AddLogging(c => c.ClearProviders());
        const string template = "[time={timestamp} | level={level} | machine={machine}] message=\"{message}\"";
        services.AddLogging(builder => builder.AddConsoleLogger(config =>
        {
            config.OutputTemplate = template;
            config.MinLevel = LogLevel.None;
        }));
        return services;
    }

    public static IServiceCollection AddFormatter(this IServiceCollection services)
    {
        services.AddTransient<IOutputFormatter, OutputFormatter>();
        return services;
    }

    public static IServiceCollection AddVersion(this IServiceCollection services)
    {
        services.AddTransient<IVersionHandler, VersionHandler>();
        return services;
    }

    public static IServiceCollection AddGitHub(this IServiceCollection services)
    {
        services.AddTransient<IGitHub, GitHub>();
        return services;
    }

    public static IServiceCollection AddExtractor(this IServiceCollection services)
    {
        services.AddTransient<IExtractor, Extractor>();
        return services;
    }
}