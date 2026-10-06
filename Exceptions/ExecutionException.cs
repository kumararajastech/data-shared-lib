namespace DataSharedLib.Exceptions;

public class ExecutionException : DatabaseException
{
    public string SqlStatement { get; }

    public ExecutionException(string message, string sqlStatement, int? sqlErrorNumber = null, Exception? innerException = null)
        : base(message, sqlErrorNumber, innerException)
    {
        SqlStatement = sqlStatement;
    }
}
