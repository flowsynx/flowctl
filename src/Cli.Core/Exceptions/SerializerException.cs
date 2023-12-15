namespace Cli.Core.Exceptions;

public class SerializerException : Exception
{
    public SerializerException(string message) : base(message) { }
    public SerializerException(string message, Exception inner) : base(message, inner) { }
}