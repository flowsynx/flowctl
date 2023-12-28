namespace FlowSynx.Cli.Formatter;

public interface ISpinner
{
    Task DisplayLineSpinnerAsync(Func<Task> func);
}