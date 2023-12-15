using Cli.Exceptions;
using Newtonsoft.Json;

namespace Cli.Serialization;

public class NewtonsoftDeserializer : IDeserializer
{
    public string ContentMineType => "application/json";

    public T? Deserialize<T>(string? input)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(input)) return JsonConvert.DeserializeObject<T>(input);
            throw new DeserializerException("NewtonsoftDeserializerValueCanNotBeEmpty");
        }
        catch (Exception ex)
        {
            throw new DeserializerException(ex.Message);
        }
    }
}