using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;

namespace FlowCtl.Commands;

public abstract class BaseCommand : Command
{
    protected BaseCommand(string name, string description) : base(name, description) { }
}

public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : class, ICommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description) : base(name, description)
    {
        this.Handler = CommandHandler.Create<TOptions, IServiceProvider, CancellationToken>(HandleOptions);
    }

    private static async Task<int> HandleOptions(TOptions options, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(serviceProvider);
        return await handler.HandleAsync(options, cancellationToken);
    }
}