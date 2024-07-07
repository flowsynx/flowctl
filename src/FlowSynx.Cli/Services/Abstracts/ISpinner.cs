namespace FlowCtl.Services.Abstracts;

public interface ISpinner
{
    Task DisplayLineSpinnerAsync(Func<Task> func);
}