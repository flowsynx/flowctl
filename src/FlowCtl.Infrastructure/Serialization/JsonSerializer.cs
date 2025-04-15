using FlowCtl.Core.Exceptions;
using FlowCtl.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlowCtl.Infrastructure.Serialization;

public class JsonSerializer : IJsonSerializer
{
    public string ContentMineType => "application/json";

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
                throw new FlowCtlException((int)ErrorCode.Serialization, "Input value can't be empty or null.");
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
            var errorMessage = new ErrorMessage((int)ErrorCode.Serialization, ex.Message);
            throw new FlowCtlException(errorMessage);
        }
    }
}