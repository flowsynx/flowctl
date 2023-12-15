namespace Cli.Exceptions;

public class HttpHandlerException : Exception
{
    public HttpHandlerException(string message) : base(message) { }
    public HttpHandlerException(string message, Exception inner) : base(message, inner) { }
}