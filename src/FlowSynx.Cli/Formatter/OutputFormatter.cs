using System.Text.RegularExpressions;
using FlowSynx.IO.Serialization;
using Spectre.Console;

namespace FlowSynx.Cli.Formatter;

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
        _console.MarkupLineInterpolated($"[red]{message}[/]");
    }

    public void WriteError(object data)
    {
        var json = _serializer.Serialize(data, new JsonSerializationConfiguration() { Indented = true });
        _console.MarkupLineInterpolated($"[red]{json}[/]");
    }

    public void Write(string message)
    {
        _console.MarkupLineInterpolated($"{message}");
    }

    public void Write(object data, Output output = Output.Json)
    {
        switch (output)
        {
            case Output.Table:
                var table = ConvertJsonToDataTable(_serializer.Serialize(data));
                table.Border = TableBorder.Simple;
                _console.Write(table);
                break;
            case Output.Json:
            default:
                var json = _serializer.Serialize(data, new JsonSerializationConfiguration() { Indented = true });
                Write(json);
                break;
        }
    }

    private Table ConvertJsonToDataTable(string json)
    {
        Table dataTable = new();
        if (string.IsNullOrWhiteSpace(json))
        {
            return dataTable;
        }
        var cleanedJson = Regex.Replace(json, "\\\\\n|\r|\t|\\[|\\]|\"", "");
        var items = Regex.Split(cleanedJson, "},{").AsSpan();
        for (var i = 0; i < items.Length; i++)
        {
            items[i] = items[i].Replace("{", "").Replace("}", "");
        }
        var columns = Regex.Split(items[0], ",").AsSpan();
        foreach (var column in columns)
        {
            var parts = Regex.Split(column, ":").AsSpan();
            dataTable.AddColumns(parts[0].Trim());
        }
        foreach (var t1 in items)
        {
            var r = dataTable.Rows;
            var values = Regex.Split(t1, ",").AsSpan();
            var rows = new List<string>(items.Length);
            foreach (var t in values)
            {
                var parts = Regex.Split(t, ":").AsSpan();
                rows.Add(int.TryParse(parts[1].Trim(), out var temp) ? temp.ToString() : parts[1].Trim());
            }
            dataTable.AddRow(rows.ToArray());
        }
        return dataTable;
    }
}