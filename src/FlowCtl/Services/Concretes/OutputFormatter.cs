using System.Reflection;
using System.Xml.Serialization;
using Spectre.Console;
using Newtonsoft.Json;
using System.Xml.Linq;
using FlowSynx.IO.Serialization;
using FlowSynx.Reflections;
using FlowCtl.Services.Abstracts;

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

    public void Write<T>(T? data, Output output = Output.Json)
    {
        switch (output)
        {
            case Output.Table:
                var table = GenerateTable(data);
                table.Border = TableBorder.Simple;
                _console.Write(table);
                break;
            case Output.Xml:
                var xml = GenerateXml(data);
                Write(xml);
                break;
            case Output.Yaml:
                var yaml = GenerateYaml(data);
                Write(yaml);
                break;
            case Output.Json:
            default:
                var json = GenerateJson(data);
                Write(json);
                break;
        }
    }

    public void Write<T>(IEnumerable<T>? data, Output output = Output.Json)
    {
        var list = data != null ? data.ToList() : new List<T>();
        Write(list, output);
    }

    public void Write<T>(List<T>? data, Output output = Output.Json)
    {
        switch (output)
        {
            case Output.Table:
                var table = GenerateTable(data);
                table.Border = TableBorder.Simple;
                _console.Write(table);
                break;
            case Output.Xml:
                var xml = GenerateXml(data);
                Write(xml);
                break;
            case Output.Yaml:
                var yaml = GenerateYaml(data);
                Write(yaml);
                break;
            case Output.Json:
            default:
                var json = GenerateJson(data);
                Write(json);
                break;
        }
    }

    private Table GenerateTable<T>(T? data)
    {
        var dataTable = new Table();
        var properties = typeof(T).Properties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
            dataTable.AddColumns(property.Name);

        var values = new List<string>(properties.Count());
        foreach (var property in properties)
        {
            var value = property.GetPropertyValue(data);
            var isDict = property.IsGenericType() && property.IsDictionaryType();
            if (isDict)
            {
                var keyValues = new List<string>();
                if (value is Dictionary<string, string> dict)
                    keyValues = dict.Select(item => $"{item.Key}={item.Value.ToString()}").ToList();

                values.Add(string.Join(System.Environment.NewLine, keyValues));
            }
            else
            {
                var val = value is null ? string.Empty : value.ToString();
                values.Add(val ?? string.Empty);
            }
        }
        dataTable.AddRow(values.ToArray());

        return dataTable;
    }

    private Table GenerateTable<T>(List<T>? data)
    {
        var dataTable = new Table();
        var properties = typeof(T).Properties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
            dataTable.AddColumns(property.Name);

        if (data is null)
            return dataTable;

        foreach (var item in data)
        {
            var values = new List<string>(properties.Count);
            foreach (var property in properties)
            {
                var value = property.GetPropertyValue(item);
                var isDict = property.IsGenericType() && property.IsDictionaryType();
                if (isDict)
                {
                    var keyValues = new List<string>();
                    if (value is Dictionary<string, string> dict)
                        keyValues = dict.Select(keyValuePair => $"{keyValuePair.Key}={keyValuePair.Value.ToString().EscapeMarkup()}").ToList();

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

    public string GenerateXml<T>(T? data)
    {
        if (data == null)
            return string.Empty;

        var xmlns = new XmlSerializerNamespaces();
        xmlns.Add(string.Empty, string.Empty);

        var json = _serializer.Serialize(data);
        var xml = JsonConvert.DeserializeXNode(json, "root");
        return xml == null ? string.Empty : xml.ToString(SaveOptions.None);
    }

    public string GenerateXml<T>(List<T>? data)
    {
        if (data == null)
            return string.Empty;

        var xmlns = new XmlSerializerNamespaces();
        xmlns.Add(string.Empty, string.Empty);

        var json = _serializer.Serialize(data);
        var xml = JsonConvert.DeserializeXNode("{item:" + json + "}", "root");
        return xml == null ? string.Empty : xml.ToString(SaveOptions.None);
    }

    public string GenerateYaml<T>(T? data)
    {
        if (data == null)
            return string.Empty;

        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(data);
    }

    public string GenerateYaml<T>(List<T>? data)
    {
        if (data == null)
            return string.Empty;

        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(data);
    }

    public string GenerateJson<T>(T? data)
    {
        if (data == null)
            return string.Empty;

        return _serializer.Serialize(data, new JsonSerializationConfiguration() { Indented = true });
    }

    public string GenerateJson<T>(List<T>? data)
    {
        if (data == null)
            return string.Empty;

        return _serializer.Serialize(data, new JsonSerializationConfiguration() { Indented = true });
    }
}