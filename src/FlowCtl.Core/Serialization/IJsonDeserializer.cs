namespace FlowCtl.Core.Serialization;

public interface IJsonDeserializer
{
    T Deserialize<T>(string? input);
    T Deserialize<T>(string input, JsonSerializationConfiguration configuration);
}