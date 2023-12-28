namespace FlowSynx.Cli.Formatter;

public interface IOutputFormatter
{
    void WriteError(string message);
    void WriteError(object data);
    void Write(string message);
    void Write(object data, Output output = Output.Json);
}