namespace FlowCtl.Core.Exceptions;

public class ErrorMessage
{
    private const string PrefixErrorMessage = "FCL"; // Abbreviation for FlowCtl

    public int Code { get; }
    public string Message { get; }

    public ErrorMessage(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public override string ToString() => $"[{PrefixErrorMessage}{Code}] {Message}";
}
