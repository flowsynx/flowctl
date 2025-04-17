using System.Diagnostics;

namespace FlowCtl.Infrastructure.Services.ProcessHost;

public interface IProcessProvider
{
    IProcessWrapper[] GetProcessesByName(string processName, string machineAddress);
}