using FlowCtl.Core.Authentication;
using FlowCtl.Core.Github;
using FlowCtl.Core.Services;
using FlowCtl.Infrastructure.Github;
using FlowCtl.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace FlowCtl.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
                .AddSingleton(new GitHubClient(new ProductHeaderValue("flowsynx")))
                .AddSingleton<IGitHubReleaseManager, GitHubReleaseManager>()
                .AddScoped<IProcessHandler, ProcessHandler>()
                .AddScoped<IArchiveExtractor, ArchiveExtractor>()
                .AddScoped<IAuthenticationManager, AuthenticationManager>();

        return services;
    }
}