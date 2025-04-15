namespace FlowCtl.Core.Exceptions;

public class FlowCtlException : Exception
{
    public int ErrorCode { get; }
    public string ErrorMessage { get; }

    public FlowCtlException(ErrorMessage errorMessage) : this(errorMessage.Code, errorMessage.Message)
    {

    }

    public FlowCtlException(int errorCode, string errorMessage) : base(errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public FlowCtlException(int errorCode, string errorMessage, Exception innerException) : base(errorMessage, innerException)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public override string ToString() => new ErrorMessage(ErrorCode, ErrorMessage).ToString();
}