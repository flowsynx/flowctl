using FlowCtl.Core.Exceptions;
using FlowCtl.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlowCtl.Infrastructure.Serialization;

public class JsonDeserializer : IJsonDeserializer
{
    public T Deserialize<T>(string? input)
    {
        return Deserialize<T>(input, new JsonSerializationConfiguration { });
    }

    public T Deserialize<T>(string? input, JsonSerializationConfiguration configuration)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FlowCtlException(Resources.JsonDeserializer_InputValueCanNotBeEmpty);
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = configuration.Indented ? Formatting.Indented : Formatting.None,
                ContractResolver = configuration.NameCaseInsensitive 
                                    ? new DefaultContractResolver() 
                                    : new CamelCasePropertyNamesContractResolver()
            };

            if (configuration.Converters is not null)
                settings.Converters = configuration.Converters.ConvertAll(item => (JsonConverter)item);

            return JsonConvert.DeserializeObject<T>(input, settings);
        }
        catch (Exception ex)
        {
            throw new FlowCtlException(ex.Message);
        }
    }
}