using System.Collections;

namespace Cli.Services;

public interface IEnvironmentVariablesManager
{
    string? Get(string variableName);
    void Set(string variableName, string value);
}