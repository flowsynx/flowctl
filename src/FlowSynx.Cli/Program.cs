using Microsoft.Extensions.DependencyInjection;
using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Cli.Extensions;
using FlowSynx.Cli.Formatter;
using FlowSynx.Net;

IServiceCollection serviceCollection = new ServiceCollection()
    .AddLoggingService()
    .AddApplication()
    .AddCommands()
    .AddFormatter()
    .AddVersion()
    .AddHttpClient()
    .AddHttpRequestService();

IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

try
{
    var cli = serviceProvider.GetService<ICliApplicationBuilder>();

    if (cli == null)
        throw new Exception("Something wrong happen during execute the application");

    return await cli.RunAsync(args);
}
catch (Exception ex)
{
    var formatter = serviceProvider.GetService<IOutputFormatter>();
    formatter?.WriteError(ex.Message);
    return 0;
}