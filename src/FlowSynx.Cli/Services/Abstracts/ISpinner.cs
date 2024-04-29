namespace FlowSynx.Cli.Services.Abstracts;

public interface ISpinner
{
    Task DisplayLineSpinnerAsync(Func<Task> func);
}