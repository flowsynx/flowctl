﻿using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Commands.Uninstall;

internal class UninstallCommand : BaseCommand<UninstallCommandOptions, UninstallCommandOptionsHandler>
{
    public UninstallCommand() : base("uninstall", "Uninstalling FlowSynx system and Cli from the current user profile and machine")
    {
        var forceOption = new Option<bool>(new[] { "--force" }, getDefaultValue :() => false, description: "Force terminate FlowSynx system if it is running");

        AddOption(forceOption);
    }
}

internal class UninstallCommandOptions : ICommandOptions
{
    public bool Force { get; set; }
}

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly ILocation _location;

    public UninstallCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(location, nameof(location));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _location = location;
    }

    private string UserProfilePath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
    private string DefaultFlowSynxDirName => Path.Combine(UserProfilePath, ".flowsynx");

    public async Task<int> HandleAsync(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(() => Execute(options, cancellationToken));
        return 0;
    }

    private Task Execute(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write("Beginning uninstalling...");

            if (options.Force)
            {
                TerminateProcess("FlowSynx", ".");
            }
            else
            {
                if (IsProcessRunning("FlowSynx", "."))
                {
                    _outputFormatter.Write("The FlowSynx engine is running. Please stop it by run the command: 'Synx stop', and try uninstall again.");
                    return Task.CompletedTask;
                }
            }

            FlowSynxDestruction();
            SelfDestruction();
            _outputFormatter.Write("Uninstalling is done!");
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }

    private bool IsProcessRunning(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);
        return processes.Length != 0;
    }

    private void TerminateProcess(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);

        if (processes.Length == 0) return;

        try
        {
            foreach (var process in processes)
                process.Kill();

            _outputFormatter.Write("The FlowSynx system was stopped successfully.");
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private void FlowSynxDestruction()
    {
        if (Directory.Exists(DefaultFlowSynxDirName))
            Directory.Delete(DefaultFlowSynxDirName, true);
    }

    private void SelfDestruction()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string scriptFile = "delete.bat";
            var strPath = Path.Combine(Directory.GetCurrentDirectory(), scriptFile);
            var strExe = new FileInfo(LookupBinaryFilePath(_location.RootLocation)).Name;
            var directoryName = Path.GetDirectoryName(strPath);

            var deleteScript = string.Format(Resources.DeleteScript_Bat, strExe, scriptFile);
            StreamWriter streamWriter = new StreamWriter(strPath);
            streamWriter.Write(deleteScript);
            streamWriter.Close();

            ProcessStartInfo startInfo = new ProcessStartInfo(scriptFile)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = directoryName
            };
            
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _outputFormatter.WriteError(ex.Message);
                System.Environment.Exit(0);
            }
        }
        else
        {
            var strExe = new FileInfo(LookupBinaryFilePath(_location.RootLocation)).FullName;
            File.Delete(strExe);
        }
    }
    
    private string LookupBinaryFilePath(string path)
    {
        var binFileName = "synx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }
}