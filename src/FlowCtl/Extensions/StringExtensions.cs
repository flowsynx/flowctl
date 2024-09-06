using Newtonsoft.Json;

namespace FlowCtl.Extensions;

public static class StringExtensions
{
    public static object? JsonToObject(this string? json)
    {
        var data = string.IsNullOrEmpty(json) ? "{}" : json;
        return JsonConvert.DeserializeObject<object>(data);
    }
}