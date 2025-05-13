using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;
using FlowCtl.Infrastructure.Services.Authentication;
using Moq;

namespace FlowCtl.Infrastructure.UnitTests.Services.Authentication;

public class AuthenticationManagerTests : IDisposable
{
    private readonly Mock<IJsonSerializer> _mockSerializer;
    private readonly Mock<IJsonDeserializer> _mockDeserializer;
    private readonly Mock<IDataProtectorWrapper> _mockProtector;
    private readonly AuthenticationManager _authManager;
    private const string ConfigPath = "config.json";

    public AuthenticationManagerTests()
    {
        _mockSerializer = new Mock<IJsonSerializer>();
        _mockDeserializer = new Mock<IJsonDeserializer>();
        _mockProtector = new Mock<IDataProtectorWrapper>();

        _mockProtector.Setup(p => p.Protect(It.IsAny<string>()))
                      .Returns((string s) => $"ENC({s})");

        _mockProtector.Setup(p => p.Unprotect(It.IsAny<string>()))
                      .Returns((string s) => s.Replace("ENC(", "").Replace(")", ""));

        _authManager = new AuthenticationManager(
            _mockSerializer.Object,
            _mockDeserializer.Object,
            _mockProtector.Object
        );
    }

    public void Dispose()
    {
        if (File.Exists(ConfigPath))
            File.Delete(ConfigPath);
    }

    [Fact]
    public void LoginBasic_ShouldSaveEncryptedData()
    {
        // Arrange
        AuthenticationData? capturedData = null;

        _mockSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
               .Callback<object>(data => capturedData = data as AuthenticationData)
               .Returns("mocked-json");

        // Act
        var result = _authManager.LoginBasic("user", "pass");

        // Assert
        Assert.NotNull(capturedData);
        Assert.Equal(AuthenticationType.Basic, capturedData!.Type);
        Assert.Equal("ENC(user)", capturedData.Username);
        Assert.Equal("ENC(pass)", capturedData.Password);
    }

    [Fact]
    public void LoginBearer_ShouldSaveEncryptedTokenWithExpiry()
    {
        AuthenticationData? capturedData = null;

        _mockSerializer.Setup(s => s.Serialize(It.IsAny<object>()))
               .Callback<object>(data => capturedData = data as AuthenticationData)
               .Returns("mocked-json");

        var result = _authManager.LoginBearer("my-token");

        Assert.NotNull(capturedData);
        Assert.Equal(AuthenticationType.Bearer, capturedData!.Type);
        Assert.StartsWith("ENC(", capturedData.AccessToken);
        Assert.True(capturedData.Expiry > DateTime.UtcNow);
    }

    [Fact]
    public void GetData_ShouldReturnDecryptedData()
    {
        var stored = new AuthenticationData
        {
            Type = AuthenticationType.Basic,
            Username = "ENC(user)",
            Password = "ENC(pass)"
        };

        File.WriteAllText(ConfigPath, "mocked-json");
        _mockDeserializer.Setup(d => d.Deserialize<AuthenticationData>("mocked-json")).Returns(stored);

        var result = _authManager.GetData();

        Assert.NotNull(result);
        Assert.Equal("user", result!.Username);
        Assert.Equal("pass", result.Password);
    }

    [Fact]
    public void IsLoggedIn_ShouldReturnTrueIfBasic()
    {
        var stored = new AuthenticationData
        {
            Type = AuthenticationType.Basic,
            Username = "ENC(user)",
            Password = "ENC(pass)"
        };

        File.WriteAllText(ConfigPath, "mocked-json");
        _mockDeserializer.Setup(d => d.Deserialize<AuthenticationData>("mocked-json")).Returns(stored);

        Assert.True(_authManager.IsLoggedIn);
    }

    [Fact]
    public void IsLoggedIn_ShouldReturnFalseIfExpiredBearer()
    {
        var stored = new AuthenticationData
        {
            Type = AuthenticationType.Bearer,
            Expiry = DateTime.UtcNow.AddHours(-1),
            AccessToken = "ENC(token)"
        };

        File.WriteAllText(ConfigPath, "mocked-json");
        _mockDeserializer.Setup(d => d.Deserialize<AuthenticationData>("mocked-json")).Returns(stored);

        Assert.False(_authManager.IsLoggedIn);
    }

    [Fact]
    public void Logout_ShouldDeleteConfigFile()
    {
        File.WriteAllText(ConfigPath, "dummy");

        Assert.True(File.Exists(ConfigPath));

        _authManager.Logout();

        Assert.False(File.Exists(ConfigPath));
    }
}