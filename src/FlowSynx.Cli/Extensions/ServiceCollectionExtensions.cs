using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using FlowSynx.Cli.Commands.Storage;
using FlowSynx.Cli.Commands.Execute;
using Spectre.Console;
using FlowSynx.Cli.Commands;
using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Cli.Commands.Config;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO;
using FlowSynx.Net;
using FlowSynx.Logging;
using Microsoft.Extensions.Logging;
using FlowSynx.Cli.Commands.Plugins;
using FlowSynx.Cli.Commands.Version;
using FlowSynx.Cli.Commands.Health;

namespace FlowSynx.Cli.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<RootCommand, Root>();
        services.AddTransient<Command, RunCommand>();
        services.AddTransient<Command, StorageCommand>();
        services.AddTransient<Command, ConfigCommand>();
        services.AddTransient<Command, PluginsCommand>();
        services.AddTransient<Command, VersionCommand>();
        services.AddTransient<Command, HealthCommand>();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddEnvironmentManager();
        services.AddEndpoint();
        services.AddSerialization();
        services.AddHttpRequestService();
        services.AddSingleton(AnsiConsole.Console);
        services.AddTransient<ICliApplicationBuilder, CliApplicationBuilder>();
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
        services.AddTransient<ISpinner, Formatter.Spinner>();
        services.AddTransient<IOutputFormatter, OutputFormatter>();
        return services;
    }

    public static IServiceCollection AddVersion(this IServiceCollection services)
    {
        services.AddTransient<IVersion, CliVersion>();
        return services;
    }
}