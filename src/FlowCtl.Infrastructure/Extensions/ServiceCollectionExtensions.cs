using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Extractor;
using FlowCtl.Core.Services.Github;
using FlowCtl.Core.Services.ProcessHost;
using FlowCtl.Infrastructure.Services.Authentication;
using FlowCtl.Infrastructure.Services.Extractor;
using FlowCtl.Infrastructure.Services.Github;
using FlowCtl.Infrastructure.Services.ProcessHost;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace FlowCtl.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDataProtection();
        services
                .AddSingleton<IGitHubClient>(new GitHubClient(new ProductHeaderValue("flowsynx")))
                .AddSingleton<IGitHubReleaseManager, GitHubReleaseManager>()
                .AddScoped<IProcessProvider, DefaultProcessProvider>()
                .AddScoped<IProcessHandler, ProcessHandler>()
                .AddScoped<IArchiveExtractor, ArchiveExtractor>()
                .AddScoped<IDataProtectorWrapper, DataProtectorWrapper>()
                .AddScoped<IAuthenticationManager, AuthenticationManager>();

        return services;
    }
}