﻿using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace FlowCtl.Infrastructure.Services.Authentication;

public class AuthenticationManager : IAuthenticationManager
{
    private const string ConfigPath = "config.json";
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IJsonDeserializer _jsonDeserializer;
    private readonly IDataProtector _protector;

    public AuthenticationManager(
        IJsonSerializer jsonSerializer,
        IJsonDeserializer jsonDeserializer,
        IDataProtectionProvider dataProtectionProvider)
    {
        _jsonSerializer = jsonSerializer;
        _jsonDeserializer = jsonDeserializer;
        _protector = dataProtectionProvider.CreateProtector("AuthenticationManager.Protector");
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

            if (data?.Type == AuthenticationType.Basic && data.Password != null)
                data.Password = _protector.Unprotect(data.Password);
            
            if (data?.Type == AuthenticationType.Bearer && data.AccessToken != null)
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