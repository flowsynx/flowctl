namespace FlowCtl.Core.Serialization;

public interface IJsonSerializer
{
    string Serialize(object? input);
    string Serialize(object? input, JsonSerializationConfiguration configuration);
}