namespace FlowSynx.Cli.Services;

public interface ISpinner
{
    Task DisplayLineSpinnerAsync(Func<Task> func);
}