namespace FlowCtl.Core.Services;

public interface IProcessHandler
{
    bool IsRunning(string processName, string machineAddress);
    void Terminate(string processName, string machineAddress);
    bool IsStopped(string processName, string machineAddress, bool force);
}