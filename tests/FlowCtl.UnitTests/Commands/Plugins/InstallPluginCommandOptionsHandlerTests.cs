using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlowCtl.Commands.Plugins.Install;
using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowSynx.Client;
using FlowSynx.Client.Authentication;
using FlowSynx.Client.Messages.Requests.Plugins;
using FlowSynx.Client.Messages.Responses;
using FlowSynx.Client.Services;
using Moq;

namespace FlowCtl.UnitTests.Commands.Plugins;

public class InstallPluginCommandOptionsHandlerTests
{
    [Fact]
    public async Task HandleAsync_DefaultsToLatestVersion_WhenVersionIsMissing()
    {
        // Arrange
        var loggerMock = new Mock<IFlowCtlLogger>();
        var pluginsServiceMock = new Mock<IPluginsService>();
        var flowSynxClientMock = new Mock<IFlowSynxClient>();
        var authenticationManagerMock = CreateAuthenticationManagerMock();

        flowSynxClientMock.Setup(client => client.Plugins)
            .Returns(pluginsServiceMock.Object);
        flowSynxClientMock.Setup(client => client.SetAuthenticationStrategy(It.IsAny<IAuthenticationStrategy>()));

        pluginsServiceMock.Setup(service => service.InstallAsync(
                It.Is<InstallPluginRequest>(request => request.Version == "latest"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSuccessfulInstallResult());

        var handler = new InstallPluginCommandOptionsHandler(
            loggerMock.Object,
            flowSynxClientMock.Object,
            authenticationManagerMock.Object);

        var options = new InstallPluginCommandOptions
        {
            Type = "email-sender",
            Version = null
        };

        // Act
        await handler.HandleAsync(options, CancellationToken.None);

        // Assert
        pluginsServiceMock.Verify(service => service.InstallAsync(
            It.Is<InstallPluginRequest>(request => request.Version == "latest"),
            It.IsAny<CancellationToken>()), Times.Once);

        loggerMock.Verify(logger => logger.Write(
                It.Is<string>(message => message.Contains("latest", StringComparison.OrdinalIgnoreCase))),
            Times.Once);
    }

    private static Mock<IAuthenticationManager> CreateAuthenticationManagerMock()
    {
        var mock = new Mock<IAuthenticationManager>();
        mock.SetupGet(manager => manager.IsLoggedIn).Returns(true);
        mock.SetupGet(manager => manager.IsBasicAuthenticationUsed).Returns(false);
        mock.Setup(manager => manager.GetData())
            .Returns(new AuthenticationData { AccessToken = "token" });
        return mock;
    }

    private static HttpResult<Result<Unit>> CreateSuccessfulInstallResult() =>
        new()
        {
            StatusCode = 200,
            Payload = new Result<Unit>
            {
                Succeeded = true,
                Data = new Unit(),
                Messages = new List<string>()
            }
        };
}
