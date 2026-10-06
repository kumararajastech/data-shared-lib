namespace DataSharedLib.Exceptions;

public class ConnectionException : DatabaseException
{
    public ConnectionException(string message) : base(message) { }
    public ConnectionException(string message, Exception innerException) : base(message, innerException) { }
}
