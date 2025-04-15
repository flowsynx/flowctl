using FlowCtl.Core.Logger;
using FlowCtl.Core.Services;
using System.Diagnostics;

namespace FlowCtl.Infrastructure.Services;

public class ProcessHandler : IProcessHandler
{
    private readonly IFlowCtlLogger _flowCtlLogger;

    public ProcessHandler(IFlowCtlLogger flowCtlLogger)
    {
        _flowCtlLogger = flowCtlLogger;
    }

    public bool IsRunning(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);
        return processes.Length != 0;
    }

    public void Terminate(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);
        if (processes.Length == 0) return;
        foreach (var process in processes)
        {
            _flowCtlLogger.Write($" Process '{process.ProcessName}' killed.");
            process.Kill();
        }
    }

    public bool IsStopped(string processName, string machineAddress, bool force)
    {
        if (!IsRunning(processName, machineAddress)) 
            return true;

        if (!force) 
            return false;

        Terminate(processName, machineAddress);
        return true;
    }
}