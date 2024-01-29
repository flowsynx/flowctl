using System.Diagnostics;

namespace FlowSynx.Cli.Common;

internal static class ProcessHelper
{
    public static bool IsProcessRunning(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);
        return processes.Length != 0;
    }

    public static void TerminateProcess(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);
        if (processes.Length == 0) return;
        foreach (var process in processes)
        {
            process.Kill();
        }
    }
}