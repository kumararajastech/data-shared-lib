namespace DataSharedLib.Exceptions;

public class DatabaseException : Exception
{
    public int? SqlErrorNumber { get; }

    public DatabaseException(string message, Exception? innerException = null, int? sqlErrorNumber = null)
        : base(message, innerException)
    {
        SqlErrorNumber = sqlErrorNumber;
    }
}
