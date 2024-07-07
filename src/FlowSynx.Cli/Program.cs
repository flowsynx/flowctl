using FlowCtl;
using Microsoft.Extensions.DependencyInjection;
using FlowCtl.ApplicationBuilders;
using FlowCtl.Extensions;
using FlowCtl.Services.Abstracts;

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
        throw new Exception(Resources.SomethingWrongHappen);

    return await cli.RunAsync(args);
}
catch (Exception ex)
{
    var formatter = serviceProvider.GetService<IOutputFormatter>();
    formatter?.WriteError(ex.Message);
    return 0;
}