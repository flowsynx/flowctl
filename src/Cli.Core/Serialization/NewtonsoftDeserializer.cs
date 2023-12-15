using Cli.Core.Exceptions;
using Newtonsoft.Json;

namespace Cli.Core.Serialization.Json;

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