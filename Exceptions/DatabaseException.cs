namespace DataSharedLib.Exceptions;

public class DatabaseException : Exception
{
    public int? SqlErrorNumber { get; }

    public DatabaseException(string message, int? sqlErrorNumber = null, Exception? innerException = null)
        : base(message, innerException)
    {
        SqlErrorNumber = sqlErrorNumber;
    }
}
