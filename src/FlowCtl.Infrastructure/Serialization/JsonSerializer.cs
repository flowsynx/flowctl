using FlowCtl.Core.Exceptions;
using FlowCtl.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlowCtl.Infrastructure.Serialization;

public class JsonSerializer : IJsonSerializer
{
    /// <summary>
    /// Static JSON content type reference that callers can reuse without instantiating the serializer.
    /// </summary>
    public static string ContentMineType => "application/json";

    public string Serialize(object? input)
    {
        return Serialize(input, new JsonSerializationConfiguration { Indented = false });
    }

    public string Serialize(object? input, JsonSerializationConfiguration configuration)
    {
        try
        {
            if (input is null)
            {
                throw new FlowCtlException(Resources.JsonSerializer_InputValueCanNotBeEmpty);
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

            return JsonConvert.SerializeObject(input, settings);
        }
        catch (Exception ex)
        {
            throw new FlowCtlException(ex.Message);
        }
    }
}