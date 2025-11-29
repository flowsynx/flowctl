using System.Diagnostics;
using System.Text;
using FlowCtl.Core.Models.Docker;
using FlowCtl.Core.Services.Docker;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Infrastructure.Services.Docker;

public class DockerService : IDockerService
{
    private readonly IFlowCtlLogger _flowCtlLogger;

    public DockerService(IFlowCtlLogger flowCtlLogger)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
    }

    public async Task<string> GetDockerModeAsync(CancellationToken cancellationToken = default)
    {
        var result = await RunDockerAsync(new[] { "info", "--format", "{{.OSType}}" }, streamOutput: false, cancellationToken);
        if (!result.Success || string.IsNullOrWhiteSpace(result.Output))
        {
            return "Unknown";
        }

        // Docker OSType usually returns "linux" or "windows"
        var osType = result.Output.Trim().ToLowerInvariant();
        return osType switch
        {
            "linux" => "Linux",
            "windows" => "Windows",
            _ => "Unknown"
        };
    }

    public Task<bool> IsDockerAvailableAsync(CancellationToken cancellationToken = default)
    {
        return ContainerQueryAsync(new[] { "info" }, cancellationToken);
    }

    public Task<DockerCommandResult> PullImageAsync(string imageName, string tag, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageName);

        var arguments = new[] { "pull", $"{imageName}:{tag}" };
        return RunDockerAsync(arguments, streamOutput: true, cancellationToken);
    }

    public Task<DockerCommandResult> RunContainerAsync(DockerRunOptions options, CancellationToken cancellationToken = default)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.ImageName))
            throw new ArgumentException("Image name is required.", nameof(options.ImageName));

        if (string.IsNullOrWhiteSpace(options.Tag))
            throw new ArgumentException("Image tag is required.", nameof(options.Tag));

        if (string.IsNullOrWhiteSpace(options.ContainerName))
            throw new ArgumentException("Container name is required.", nameof(options.ContainerName));

        if (string.IsNullOrWhiteSpace(options.HostDataPath))
            throw new ArgumentException("Host data path is required.", nameof(options.HostDataPath));

        if (string.IsNullOrWhiteSpace(options.ContainerDataPath))
            throw new ArgumentException("Container data path is required.", nameof(options.ContainerDataPath));

        Directory.CreateDirectory(options.HostDataPath);

        var arguments = new List<string> { "run" };
        if (options.Detached)
            arguments.Add("-d");

        arguments.Add("--name");
        arguments.Add(options.ContainerName);

        arguments.Add("-p");
        arguments.Add($"{options.HostPort}:{options.ContainerPort}");

        arguments.Add("-v");
        arguments.Add($"{options.HostDataPath}:{options.ContainerDataPath}");

        arguments.Add($"{options.ImageName}:{options.Tag}");

        if (!string.IsNullOrWhiteSpace(options.AdditionalArguments))
        {
            arguments.AddRange(options.AdditionalArguments.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        return RunDockerAsync(arguments, streamOutput: true, cancellationToken);
    }

    public Task<DockerCommandResult> StartContainerAsync(string containerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);
        return RunDockerAsync(new[] { "start", containerName }, streamOutput: true, cancellationToken);
    }

    public Task<DockerCommandResult> StopContainerAsync(string containerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);
        return RunDockerAsync(new[] { "stop", containerName }, streamOutput: true, cancellationToken);
    }

    public Task<DockerCommandResult> RemoveContainerAsync(string containerName, bool force, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);

        var arguments = new List<string> { "rm" };
        if (force)
            arguments.Add("-f");

        arguments.Add(containerName);
        return RunDockerAsync(arguments, streamOutput: true, cancellationToken);
    }

    public Task<bool> ContainerExistsAsync(string containerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);

        var arguments = new[]
        {
            "ps",
            "-a",
            "--filter",
            $"name={containerName}",
            "--format",
            "{{.ID}}"
        };

        return ContainerQueryAsync(arguments, cancellationToken);
    }

    public Task<bool> IsContainerRunningAsync(string containerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);

        var arguments = new[]
        {
            "ps",
            "--filter",
            $"name={containerName}",
            "--filter",
            "status=running",
            "--format",
            "{{.ID}}"
        };

        return ContainerQueryAsync(arguments, cancellationToken);
    }

    public Task<DockerCommandResult> TailLogsAsync(string containerName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);
        return RunDockerAsync(new[] { "logs", "-f", containerName }, streamOutput: true, cancellationToken);
    }

    private async Task<bool> ContainerQueryAsync(IEnumerable<string> arguments, CancellationToken cancellationToken)
    {
        var result = await RunDockerAsync(arguments, streamOutput: false, cancellationToken);
        return result.Success && !string.IsNullOrWhiteSpace(result.Output);
    }

    private async Task<DockerCommandResult> RunDockerAsync(IEnumerable<string> arguments, bool streamOutput, CancellationToken cancellationToken)
    {
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();
        using var process = new Process();

        try
        {
            process.StartInfo.FileName = "docker";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            foreach (var argument in arguments)
                process.StartInfo.ArgumentList.Add(argument);

            process.OutputDataReceived += (_, data) =>
            {
                if (data.Data is null) return;

                outputBuilder.AppendLine(data.Data);
                if (streamOutput)
                    _flowCtlLogger.Write(data.Data);
            };

            process.ErrorDataReceived += (_, data) =>
            {
                if (data.Data is null) return;

                errorBuilder.AppendLine(data.Data);
                if (streamOutput)
                    _flowCtlLogger.WriteError(data.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);
            return new DockerCommandResult
            {
                ExitCode = process.ExitCode,
                Output = outputBuilder.ToString().Trim(),
                Error = errorBuilder.ToString().Trim()
            };
        }
        catch (OperationCanceledException)
        {
            TryKill(process);
            return new DockerCommandResult { ExitCode = -1, Error = "Operation cancelled." };
        }
        catch (Exception ex)
        {
            TryKill(process);
            return new DockerCommandResult { ExitCode = -1, Error = ex.Message };
        }
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
                process.Kill(true);
        }
        catch
        {
            // Swallow exceptions; best-effort cleanup only.
        }
    }
}
