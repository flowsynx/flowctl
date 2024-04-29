using FlowSynx.Cli.Services.Abstracts;
using Spectre.Console;

namespace FlowSynx.Cli.Services.Concretes;

public class Spinner : ISpinner
{
    private readonly IAnsiConsole _console;

    public Spinner(IAnsiConsole console)
    {
        _console = console;
    }

    public async Task DisplayLineSpinnerAsync(Func<Task> func)
    {
        await _console.Status()
            .AutoRefresh(true)
            .StartAsync("Fetching...", async ctx =>
            {
                ctx.Spinner(Spectre.Console.Spinner.Known.Line);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                await func();
            });
    }
}