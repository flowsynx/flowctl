using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Cli.Commands.Storage;
using Cli.Commands.Execute;
using Cli.Serialization;
using Cli.Services;
using Spectre.Console;
using Cli.Commands;
using Cli.ApplicationBuilders;

namespace Cli.Extensions;

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
        services.AddTransient<IEnvironmentVariablesManager, EnvironmentVariablesManager>();
        services.AddTransient<IFlowSyncDefaultEndpoint, FlowSyncDefaultEndpoint>();
        services.AddTransient<ISerializer, NewtonsoftSerializer>();
        services.AddTransient<IDeserializer, NewtonsoftDeserializer>();
        services.AddScoped<IHttpHandler, HttpHandler>();
        services.AddSingleton(AnsiConsole.Console);
        services.AddTransient<ICliApplicationBuilder, CliApplicationBuilder>();
        return services;
    }
}