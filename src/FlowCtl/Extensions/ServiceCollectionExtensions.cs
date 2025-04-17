using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using FlowCtl.Commands;
using FlowCtl.Commands.Config;
using FlowCtl.Commands.Plugins;
using FlowCtl.Commands.Version;
using FlowCtl.Commands.Health;
using FlowCtl.Commands.Update;
using FlowCtl.Commands.Init;
using FlowCtl.Commands.Run;
using FlowCtl.Commands.Stop;
using FlowCtl.Commands.Uninstall;
using FlowSynx.Client;
using FlowCtl.Commands.Logs;
using FlowCtl.Services;
using FlowCtl.Infrastructure.Extensions;
using FlowCtl.Core.Services;
using FlowCtl.Commands.Login;
using Spectre.Console;
using FlowCtl.Core.Serialization;
using FlowCtl.Infrastructure.Serialization;
using FlowCtl.ApplicationBuilders;
using FlowCtl.Core.Logger;
using FlowCtl.Commands.Logout;
using FlowCtl.Commands.Workflows;

namespace FlowCtl.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCancellationTokenSource(this IServiceCollection services)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        services.AddSingleton(cancellationTokenSource);
        return services;
    }

    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<RootCommand, Root>()
                .AddTransient<Command, ConfigCommand>()
                .AddTransient<Command, HealthCommand>()
                .AddTransient<Command, InitCommand>()
                .AddTransient<Command, LoginCommand>()
                .AddTransient<Command, LogoutCommand>()
                .AddTransient<Command, LogsCommand>()
                .AddTransient<Command, PluginsCommand>()
                .AddTransient<Command, RunCommand>()
                .AddTransient<Command, StopCommand>()
                .AddTransient<Command, UninstallCommand>()
                .AddTransient<Command, UpdateCommand>()
                .AddTransient<Command, VersionCommand>()
                .AddTransient<Command, WorkflowsCommand>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var cancellationTokenSource = serviceProvider.GetRequiredService<CancellationTokenSource>();

        var cancellationToken = cancellationTokenSource.Token;

        services
            .AddInfrastructure()
            .AddLogging(c => c.ClearProviders())
            .AddScoped<ILocation, Location>()
            .AddScoped<IFlowCtlLogger, SpectreConsoleLogger>()
            .AddTransient<IVersion, VersionHandler>()
            .AddScoped<IJsonSerializer, JsonSerializer>()
            .AddScoped<IJsonDeserializer, JsonDeserializer>()
            .AddTransient<ICliApplicationBuilder, CliApplicationBuilder>()
            .AddSingleton(AnsiConsole.Console)
            .AddScoped<IFlowSynxClient, FlowSynxClient>()
            .AddSingleton(new FlowSynxClientConnection());

        return services;
    }
}