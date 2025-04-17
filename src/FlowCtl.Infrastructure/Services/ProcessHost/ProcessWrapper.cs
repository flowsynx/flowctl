using System.Diagnostics;

namespace FlowCtl.Infrastructure.Services.ProcessHost;

public class ProcessWrapper : IProcessWrapper
{
    private readonly Process _process;

    public ProcessWrapper(Process process)
    {
        _process = process;
    }

    public string ProcessName => _process.ProcessName;

    public void Kill() => _process.Kill();
}