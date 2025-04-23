using System.Dynamic;
using Spectre.Console;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Services.Logger;

public class SpectreConsoleLogger : IFlowCtlLogger
{
    private readonly IAnsiConsole _console;
    private readonly IJsonSerializer _serializer;
    private readonly IJsonDeserializer _deserializer;

    public SpectreConsoleLogger(IAnsiConsole console, IJsonSerializer serializer, IJsonDeserializer deserializer)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
    }

    public void WriteError(string message) => WriteError(new { message });

    public void WriteError(object data)
    {
        var json = SerializeIndented(data);
        _console.MarkupLineInterpolated($"[red]{json.EscapeMarkup()}[/]");
    }

    public void Write(string message) => _console.MarkupLineInterpolated($"{message}");

    public void Write(object? data, OutputType output = OutputType.Json)
    {
        var json = SerializeIndented(data);

        switch (output)
        {
            case OutputType.Table:
                _console.Write(GenerateTable(json));
                break;
            case OutputType.Xml:
                Write(GenerateXml(json));
                break;
            case OutputType.Yaml:
                Write(GenerateYaml(json));
                break;
            case OutputType.Json:
            default:
                Write(json);
                break;
        }
    }

    private string SerializeIndented(object? data) =>
        _serializer.Serialize(data, new JsonSerializationConfiguration { Indented = true });

    private Table GenerateTable(string? json)
    {
        var table = new Table { Border = TableBorder.Simple };
        if (string.IsNullOrEmpty(json)) return table;

        var expandoList = DeserializeToExpandoList(json);
        if (!expandoList.Any()) return table;

        var headers = (IDictionary<string, object?>)expandoList.First();
        foreach (var header in headers.Keys)
            table.AddColumn(header);

        foreach (var item in expandoList)
        {
            var row = ((IDictionary<string, object?>)item)
                .Select(kv => FormatValue(kv.Value))
                .ToArray();
            table.AddRow(row);
        }

        return table;
    }

    private List<ExpandoObject> DeserializeToExpandoList(string json)
    {
        var config = new JsonSerializationConfiguration
        {
            Converters = new List<object> { new ExpandoObjectConverter() }
        };

        var token = JToken.Parse(json);
        if (token.Type == JTokenType.Array)
        {
            var list = _deserializer.Deserialize<List<ExpandoObject>>(json, config);
            return list ?? throw new Exception(Resources.ConsoleLogger_DataConversionError);
        }

        var single = _deserializer.Deserialize<ExpandoObject>(json, config);
        return single != null 
            ? new List<ExpandoObject> { single } 
            : throw new Exception(Resources.ConsoleLogger_DataConversionError);
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            ExpandoObject exp => FormatExpando(exp),
            null => string.Empty,
            _ => value.ToString()?.EscapeMarkup() ?? string.Empty
        };
    }

    private static string FormatExpando(ExpandoObject expando)
    {
        return string.Join(Environment.NewLine,
            from kv in (IDictionary<string, object?>)expando
            let val = kv.Value?.ToString().EscapeMarkup() ?? string.Empty
            select $"{kv.Key}={val}");
    }

    public string GenerateXml(string? data)
    {
        if (string.IsNullOrWhiteSpace(data)) return string.Empty;

        var xml = JsonConvert.DeserializeXNode($"{{item:{data}}}", "root");
        return xml?.ToString(SaveOptions.None) ?? string.Empty;
    }

    public string GenerateYaml(string? data)
    {
        if (string.IsNullOrWhiteSpace(data)) return string.Empty;

        var yamlSerializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var expandoObjects = DeserializeToExpandoList(data);
        return yamlSerializer.Serialize(expandoObjects.Count == 1 ? (object)expandoObjects[0] : expandoObjects);
    }

    public string GenerateJson(string? data) => data ?? string.Empty;
}