using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using FlowSynx.Cli.Commands.Storage;
using FlowSynx.Cli.Commands.Execute;
using FlowSynx.Cli.Commands;
using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Cli.Commands.Config;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO;
using FlowSynx.Net;
using FlowSynx.Logging;
using FlowSynx.Cli.Commands.Plugins;
using FlowSynx.Cli.Commands.Version;
using FlowSynx.Cli.Commands.Health;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocation(this IServiceCollection services)
    {
        services.AddTransient<ILocation, CliLocation>();
        return services;
    }

    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<RootCommand, Root>();
        services.AddTransient<Command, ConfigCommand>();
        services.AddTransient<Command, HealthCommand>();
        services.AddTransient<Command, InitCommand>();
        services.AddTransient<Command, PluginsCommand>();
        services.AddTransient<Command, RunCommand>();
        services.AddTransient<Command, StorageCommand>();
        services.AddTransient<Command, StopCommand>();
        services.AddTransient<Command, UninstallCommand>();
        services.AddTransient<Command, UpdateCommand>();
        services.AddTransient<Command, VersionCommand>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddOperatingSystemInfo();
        services.AddCompression();
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