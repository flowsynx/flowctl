using FlowCtl.Core.Models.Docker;

namespace FlowCtl.Core.Services.Docker;

public interface IDockerService
{
    Task<bool> IsDockerAvailableAsync(CancellationToken cancellationToken = default);
    Task<DockerCommandResult> PullImageAsync(string imageName, string tag, CancellationToken cancellationToken = default);
    Task<DockerCommandResult> RunContainerAsync(DockerRunOptions options, CancellationToken cancellationToken = default);
    Task<DockerCommandResult> StartContainerAsync(string containerName, CancellationToken cancellationToken = default);
    Task<DockerCommandResult> StopContainerAsync(string containerName, CancellationToken cancellationToken = default);
    Task<DockerCommandResult> RemoveContainerAsync(string containerName, bool force, CancellationToken cancellationToken = default);
    Task<bool> ContainerExistsAsync(string containerName, CancellationToken cancellationToken = default);
    Task<bool> IsContainerRunningAsync(string containerName, CancellationToken cancellationToken = default);
    Task<DockerCommandResult> TailLogsAsync(string containerName, CancellationToken cancellationToken = default);
}
