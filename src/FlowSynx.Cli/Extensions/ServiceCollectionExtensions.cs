using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using FlowSynx.Cli.Commands.Storage;
using FlowSynx.Cli.Commands;
using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Cli.Commands.Config;
using FlowSynx.Environment;
using FlowSynx.IO;
using FlowSynx.Logging;
using FlowSynx.Cli.Commands.Plugins;
using FlowSynx.Cli.Commands.Version;
using FlowSynx.Cli.Commands.Health;
using FlowSynx.Cli.Commands.Update;
using FlowSynx.Cli.Services;
using FlowSynx.Cli.Commands.Init;
using FlowSynx.Cli.Commands.Run;
using FlowSynx.Cli.Commands.Stop;
using FlowSynx.Cli.Commands.Uninstall;
using FlowSynx.Client;
using FlowSynx.Cli.Commands.Dashboard;

namespace FlowSynx.Cli.Extensions;

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
                .AddTransient<Command, DashboardCommand>()
                .AddTransient<Command, HealthCommand>()
                .AddTransient<Command, InitCommand>()
                .AddTransient<Command, PluginsCommand>()
                .AddTransient<Command, RunCommand>()
                .AddTransient<Command, StorageCommand>()
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
        services.AddTransient<ISpinner, Services.Spinner>()
                .AddTransient<IOutputFormatter, OutputFormatter>();

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