namespace FlowCtl.Services.Abstracts;

public interface IOutputFormatter
{
    void WriteError(string message);
    void WriteError(object data);
    void Write(string message);
    void Write(object? data, Output output = Output.Json);
}