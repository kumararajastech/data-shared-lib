namespace DataSharedLib.Exceptions;

public class ConnectionException : DatabaseException
{
    public ConnectionException(string message, Exception? innerException = null, int? sqlErrorNumber = null)
        : base(message, innerException, sqlErrorNumber)
    {
    }
}
