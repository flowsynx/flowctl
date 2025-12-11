using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using FlowCtl.Commands;
using FlowCtl.Commands.Plugins;
using FlowCtl.Commands.Version;
using FlowCtl.Commands.Health;
using FlowCtl.Commands.Update;
using FlowCtl.Commands.Init;
using FlowCtl.Commands.Run;
using FlowCtl.Commands.Stop;
using FlowCtl.Commands.Uninstall;
using FlowCtl.Commands.Logs;
using FlowCtl.Commands.Login;
using FlowCtl.Commands.Logout;
using FlowCtl.Commands.Workflows;
using FlowCtl.Core.Serialization;
using FlowCtl.Infrastructure.Serialization;
using FlowCtl.Infrastructure.Extensions;
using FlowCtl.ApplicationBuilders;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Core.Services.Location;
using FlowCtl.Services.Location;
using FlowCtl.Services.Version;
using FlowCtl.Services.Logger;
using FlowSynx.Client;
using Spectre.Console;
using FlowSynx.Client.Authentication;
using FlowCtl.Commands.Console;

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
                .AddTransient<Command, ConsoleCommand>()
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
        var connection = new FlowSynxClientConnection(FlowSynxEnvironments.GetDefaultHttpEndpoint());
        var basicAuthStrategy = new BasicAuthenticationStrategy(string.Empty, string.Empty);

        services
            .AddInfrastructure()
            .AddLogging(c => c.ClearProviders())
            .AddScoped<ILocation, LocationService>()
            .AddScoped<IFlowCtlLogger, SpectreConsoleLogger>()
            .AddScoped<IFileSystem, FileSystemWrapper>()
            .AddScoped<IVersionInfoProvider, VersionInfoProvider>()
            .AddTransient<IVersion, VersionHandler>()
            .AddScoped<IJsonSerializer, JsonSerializer>()
            .AddScoped<IJsonDeserializer, JsonDeserializer>()
            .AddTransient<ICliApplicationBuilder, CliApplicationBuilder>()
            .AddSingleton(AnsiConsole.Console)
            .AddSingleton<IFlowSynxClientConnection>(x => connection)
            .AddSingleton<IAuthenticationStrategy>(x=> basicAuthStrategy)
            .AddSingleton<IFlowSynxServiceFactory, FlowSynxServiceFactory>()
            .AddScoped<IFlowSynxClient, FlowSynxClient>();

        return services;
    }
}