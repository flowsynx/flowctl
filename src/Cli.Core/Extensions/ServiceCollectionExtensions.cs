using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Cli.Core.Services;
using Cli.Core.Serialization.Json;
using Cli.Core.Serialization;
using FlowSync.Infrastructure.Serialization.Json;

namespace Cli.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCliCore(this IServiceCollection services)
    {
        services.AddMediatR(config => {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddTransient<ISerializer, NewtonsoftSerializer>();
        services.AddTransient<IDeserializer, NewtonsoftDeserializer>();
        services.AddScoped<IHttpHandler, HttpHandler>();
        return services;
    }
}