namespace Cli.Core.Serialization;

public interface IDeserializer
{
    string ContentMineType { get;}
    T Deserialize<T>(string? input);
}