namespace DataSharedLib.Exceptions;

public class ConnectionException : DatabaseException
{
    public ConnectionException(string message, int? sqlErrorNumber = null, Exception? innerException = null)
        : base(message, sqlErrorNumber, innerException)
    {
    }
}
