using Microsoft.Extensions.DependencyInjection;
using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Cli.Extensions;
using FlowSynx.Cli.Services.Abstracts;

IServiceCollection serviceCollection = new ServiceCollection()
    .AddLocation()
    .AddLoggingService()
    .AddApplication()
    .AddCommands()
    .AddFormatter()
    .AddVersion()
    .AddGitHub()
    .AddExtractor()
    .AddHttpClient();

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