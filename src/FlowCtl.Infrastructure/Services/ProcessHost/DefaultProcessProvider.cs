using System.Diagnostics;

namespace FlowCtl.Infrastructure.Services.ProcessHost;

public class DefaultProcessProvider : IProcessProvider
{
    public IProcessWrapper[] GetProcessesByName(string processName, string machineAddress)
    {
        return Process.GetProcessesByName(processName, machineAddress)
                      .Select(p => new ProcessWrapper(p))
                      .ToArray();
    }
}