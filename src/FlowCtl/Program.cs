using FlowCtl;
using Microsoft.Extensions.DependencyInjection;
using FlowCtl.ApplicationBuilders;
using FlowCtl.Extensions;
using FlowCtl.Services;
using FlowCtl.Core.Services.Logger;

IServiceProvider serviceProvider = default!;

try
{
    IServiceCollection services = new ServiceCollection();

    services
        .AddCancellationTokenSource()
        .AddApplication()
        .AddCommands()
        .AddHttpClient();

    serviceProvider = services.BuildServiceProvider();

    var cli = serviceProvider.GetService<ICliApplicationBuilder>();

    if (cli == null)
        throw new Exception(Resources.SomethingWrongHappen);

    return await cli.RunAsync(args);
}
catch (Exception ex)
{
    if (serviceProvider != null)
    {
        var formatter = serviceProvider.GetService<IFlowCtlLogger>();
        formatter?.WriteError(ex.Message);
    }
    else
    {
        Console.WriteLine(ex.Message);
    }
    return 0;
}