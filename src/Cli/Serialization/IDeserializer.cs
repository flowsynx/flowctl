namespace Cli.Serialization;

public interface IDeserializer
{
    string ContentMineType { get;}
    T Deserialize<T>(string? input);
}