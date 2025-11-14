using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;

namespace FlowCtl.Infrastructure.Services.Authentication;

public class AuthenticationManager : IAuthenticationManager
{
    private const string ConfigPath = "config.json";
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IJsonDeserializer _jsonDeserializer;
    private readonly IDataProtectorWrapper _protector;

    public AuthenticationManager(
        IJsonSerializer jsonSerializer,
        IJsonDeserializer jsonDeserializer,
        IDataProtectorWrapper protector)
    {
        _jsonSerializer = jsonSerializer;
        _jsonDeserializer = jsonDeserializer;
        _protector = protector;
    }

    public bool IsLoggedIn => File.Exists(ConfigPath) && Load() is { } data && (
        data.Type == AuthenticationType.Basic
        || data.Type == AuthenticationType.None
        || data.Expiry is null
        || data.Expiry > DateTime.UtcNow
    );

    public bool IsBasicAuthenticationUsed => File.Exists(ConfigPath) && Load() is { } data &&
        data.Type == AuthenticationType.Basic;

    public AuthenticationData LoginNone()
    {
        var data = new AuthenticationData
        {
            Type = AuthenticationType.None,
            Username = null,
            Password = null,
            AccessToken = null,
            Expiry = null
        };
        Save(data);
        return data;
    }

    public AuthenticationData LoginBasic(string username, string password)
    {
        var data = new AuthenticationData
        {
            Type = AuthenticationType.Basic,
            Username = _protector.Protect(username),
            Password = _protector.Protect(password) // Encrypt password
        };
        Save(data);
        return data;
    }

    public AuthenticationData LoginBearer(string token)
    {
        var data = new AuthenticationData
        {
            Type = AuthenticationType.Bearer,
            AccessToken = _protector.Protect(token), // Encrypt token
            Expiry = DateTime.UtcNow.AddHours(1)
        };
        Save(data);
        return data;
    }

    public AuthenticationData? GetData()
    {
        var data = Load();

        if (data != null)
        {
            if (data.Username != null)
                data.Username = _protector.Unprotect(data.Username);

            // data is guaranteed non-null within this scope, so we can evaluate authentication type directly.
            if (data.Type == AuthenticationType.Basic && data.Password != null)
                data.Password = _protector.Unprotect(data.Password);

            if (data.Type == AuthenticationType.Bearer && data.AccessToken != null)
                data.AccessToken = _protector.Unprotect(data.AccessToken);
        }

        return data;
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