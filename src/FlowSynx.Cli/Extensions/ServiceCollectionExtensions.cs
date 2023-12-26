using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using FlowSynx.Cli.Commands.Storage;
using FlowSynx.Cli.Commands.Execute;
using Spectre.Console;
using FlowSynx.Cli.Commands;
using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Environment;
using FlowSynx.IO;
using FlowSynx.Net;

namespace FlowSynx.Cli.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<RootCommand, Root>();
        services.AddTransient<Command, RunCommand>();
        services.AddTransient<Command, StorageCommand>();
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
}