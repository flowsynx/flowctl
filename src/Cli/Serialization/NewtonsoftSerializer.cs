using Cli.Exceptions;
using Newtonsoft.Json;

namespace Cli.Serialization;

public class NewtonsoftSerializer : ISerializer
{
    public string ContentMineType => "application/json";

    public string Serialize(object? input)
    {
        try
        {
            if (input is not null) return JsonConvert.SerializeObject(input, Formatting.Indented);

            throw new SerializerException("NewtonsoftSerializerValueCanNotBeEmpty");
        }
        catch (Exception ex)
        {
            throw new SerializerException(ex.Message);
        }
    }
}