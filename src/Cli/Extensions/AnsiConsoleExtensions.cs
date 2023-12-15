using Spectre.Console;

namespace Cli.Extensions;

public static class AnsiConsoleExtensions
{
    public static void WriteError(this IAnsiConsole console, string message)
    {
        console.MarkupLineInterpolated($"[red]{message}[/]");
    }

    public static void WriteText(this IAnsiConsole console, string message)
    {
        console.MarkupLineInterpolated($"{message}");
    }

    public static async Task DisplayLineSpinnerAsync(this IAnsiConsole console, Func<Task> func)
    {
        await console.Status()
            .AutoRefresh(true)
            .StartAsync("Fetching...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Line);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                await func();
            });
    }
}