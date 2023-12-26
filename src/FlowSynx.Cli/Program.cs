using FlowSynx.Cli.ApplicationBuilders;
using FlowSynx.Cli.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

IServiceCollection serviceCollection = new ServiceCollection()
    .AddApplication()
    .AddCommands();

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
    AnsiConsole.Console.WriteError(ex.Message);
    return 0;
}