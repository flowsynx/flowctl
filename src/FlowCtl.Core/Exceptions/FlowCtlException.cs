namespace FlowCtl.Core.Exceptions;

public class FlowCtlException : Exception
{
    public FlowCtlException(string message) : base(message) { }
    public FlowCtlException(string message, Exception inner) : base(message, inner) { }
}