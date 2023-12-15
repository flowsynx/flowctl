namespace Cli.Serialization;

public interface ISerializer
{
    string ContentMineType { get;}
    string Serialize(object? input);
}