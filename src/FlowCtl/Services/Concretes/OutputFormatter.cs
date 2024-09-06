using System.Dynamic;
using Spectre.Console;
using Newtonsoft.Json;
using System.Xml.Linq;
using FlowSynx.IO.Serialization;
using FlowCtl.Services.Abstracts;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FlowCtl.Services.Concretes;

public class OutputFormatter : IOutputFormatter
{
    private readonly IAnsiConsole _console;
    private readonly ISerializer _serializer;

    public OutputFormatter(IAnsiConsole console, ISerializer serializer)
    {
        _console = console;
        _serializer = serializer;
    }

    public void WriteError(string message)
    {
        _console.MarkupLineInterpolated($"[red]{message.EscapeMarkup()}[/]");
    }

    public void WriteError(object data)
    {
        var json = _serializer.Serialize(data, new JsonSerializationConfiguration() { Indented = true });
        _console.MarkupLineInterpolated($"[red]{json.EscapeMarkup()}[/]");
    }

    public void Write(string message)
    {
        _console.MarkupLineInterpolated($"{message}");
    }

    public void Write(object? data, Output output = Output.Json)
    {
        var jsonSerializeObject = JsonConvert.SerializeObject(data, Formatting.Indented);
        switch (output)
        {
            case Output.Table:
                var table = GenerateTable(jsonSerializeObject);
                table.Border = TableBorder.Simple;
                _console.Write(table);
                break;
            case Output.Xml:
                var xml = GenerateXml(jsonSerializeObject);
                Write(xml);
                break;
            case Output.Yaml:
                var yaml = GenerateYaml(jsonSerializeObject);
                Write(yaml);
                break;
            case Output.Json:
            default:
                var json = GenerateJson(jsonSerializeObject);
                Write(json);
                break;
        }
    }

    private Table GenerateTable(string? data)
    {
        var dataTable = new Table();

        if (data is null)
            return dataTable;

        var expandoObjects = new List<ExpandoObject>();
        var expConverter = new ExpandoObjectConverter();
        var token = JToken.Parse(data);

        if (token.Type == JTokenType.Array)
        {
            var deserializedObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(data, expConverter);
            if (deserializedObject is null)
                throw new Exception("Data conversion failed.");

            expandoObjects.AddRange(deserializedObject);
        }
        else
        {
            var deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(data, expConverter);
            if (deserializedObject is null)
                throw new Exception("Data conversion failed.");

            expandoObjects.Add(deserializedObject);
        }

        if (!expandoObjects.Any())
            return dataTable;

        var properties = (IDictionary<string, object?>)expandoObjects.First();

        foreach (var property in properties)
            dataTable.AddColumns(property.Key);

        foreach (var itemExpandoObject in expandoObjects)
        {
            var values = new List<string>(properties.Count());
            var properties2 = (IDictionary<string, object?>)itemExpandoObject;
            foreach (var property in properties2)
            {
                var value = property.Value;
                var isExpandoObject = value is ExpandoObject;

                if (isExpandoObject)
                {
                    var valueDic = (IDictionary<string, object>?)value;
                    if (valueDic == null) 
                        continue;

                    var keyValues = valueDic
                        .Select(item => $"{item.Key}={item.Value.ToString().EscapeMarkup()}")
                        .ToList();

                    values.Add(string.Join(System.Environment.NewLine, keyValues));
                }
                else
                {
                    var val = value is null ? string.Empty : value.ToString().EscapeMarkup();
                    values.Add(val ?? string.Empty);
                }
            }
            dataTable.AddRow(values.ToArray());
        }

        return dataTable;
    }

    public string GenerateXml(string? data)
    {
        if (data == null)
            return string.Empty;
        
        var xml = JsonConvert.DeserializeXNode("{item:" + data + "}", "root");
        return xml == null ? string.Empty : xml.ToString(SaveOptions.None);
    }
    
    public string GenerateYaml(string? data)
    {
        if (data == null)
            return string.Empty;

        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var expConverter = new ExpandoObjectConverter();
        var token = JToken.Parse(data);

        if (token.Type == JTokenType.Array)
        {
            var deserializedObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(data, expConverter);
            return serializer.Serialize(deserializedObject);
        }
        else
        {
            var deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(data, expConverter);
            return serializer.Serialize(deserializedObject);
        }
    }

    public string GenerateJson(string? data)
    {
        return data ?? string.Empty;
    }
}