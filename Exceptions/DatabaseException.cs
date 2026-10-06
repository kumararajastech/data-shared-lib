namespace DataSharedLib.Exceptions;

public class DatabaseException : Exception
{
    public int? SqlErrorNumber { get; }

    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
    public DatabaseException(string message, int sqlErrorNumber, Exception innerException) : base(message, innerException)
    {
        SqlErrorNumber = sqlErrorNumber;
    }
}
