namespace DataSharedLib.Exceptions;

public class ExecutionException : DatabaseException
{
    public string? CommandText { get; }

    public ExecutionException(string message, string? commandText = null, Exception? innerException = null, int? sqlErrorNumber = null)
        : base(message, innerException, sqlErrorNumber)
    {
        CommandText = commandText;
    }
}
