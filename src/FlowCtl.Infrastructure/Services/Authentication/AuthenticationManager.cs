using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;

namespace FlowCtl.Infrastructure.Services.Authentication;

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
        data.Type == AuthenticationType.Basic || data.Expiry is null || data.Expiry > DateTime.UtcNow
    );

    public bool IsBasicAuthenticationUsed => File.Exists(ConfigPath) && Load() is { } data && 
        data.Type == AuthenticationType.Basic;

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