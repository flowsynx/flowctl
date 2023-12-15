using Cli.Core.Exceptions;
using Cli.Core.Serialization;
using Newtonsoft.Json;

namespace FlowSync.Infrastructure.Serialization.Json;

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