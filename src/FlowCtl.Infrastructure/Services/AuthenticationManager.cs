using Duende.IdentityModel.OidcClient;
using FlowCtl.Core.Authentication;
using FlowCtl.Core.Serialization;

namespace FlowCtl.Infrastructure.Services;

public class AuthenticationManager : IAuthenticationManager
{
    private const string ConfigPath = "config.json";
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IJsonDeserializer _jsonDeserializer;

    public AuthenticationManager(IJsonSerializer jsonSerializer, IJsonDeserializer jsonDeserializer)
    {
        _jsonSerializer = jsonSerializer;
        _jsonDeserializer = jsonDeserializer;
    }

    public bool IsLoggedIn => File.Exists(ConfigPath) && Load() is { } data && (
        data.Type == AuthenticationType.Basic || (data.Expiry is null || data.Expiry > DateTime.UtcNow)
    );

    public bool IsBasicAuthenticationUsed => File.Exists(ConfigPath) && Load() is { } data && (
        data.Type == AuthenticationType.Basic);

    public AuthenticationData LoginBasic(string username, string password)
    {
        var data = new AuthenticationData
        {
            Type = AuthenticationType.Basic,
            Username = username,
            Password = password
        };
        Save(data);
        return data;
    }

    public AuthenticationData LoginBearer(string token)
    {
        var data = new AuthenticationData
        {
            Type = AuthenticationType.Bearer,
            AccessToken = token,
            Expiry = DateTime.UtcNow.AddHours(1)
        };
        Save(data);
        return data;
    }

    public async Task<AuthenticationData> LoginOAuthAsync(string authority, string clientId, string? scope)
    {
        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = clientId,
            Scope = scope,
            RedirectUri = "http://localhost:7890/",
            Browser = new SystemBrowser(7890)
        };

        var client = new OidcClient(options);
        var result = await client.LoginAsync(new LoginRequest());

        if (result.IsError)
            throw new Exception("OAuth login failed: " + result.Error);

        var data = new AuthenticationData
        {
            Type = AuthenticationType.Bearer,
            AccessToken = result.AccessToken,
            Expiry = DateTime.UtcNow.AddSeconds(result.AccessTokenExpiration.Second)
        };

        Save(data);
        return data;
    }

    public AuthenticationData? GetData()
    {
        return Load();
    }

    public void Logout()
    {
        if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
    }

    private AuthenticationData? Load()
    {
        if (!File.Exists(ConfigPath)) return null;
        return _jsonDeserializer.Deserialize<AuthenticationData>(File.ReadAllText(ConfigPath));
    }

    private void Save(AuthenticationData data)
    {
        File.WriteAllText(ConfigPath, _jsonSerializer.Serialize(data));
    }
}