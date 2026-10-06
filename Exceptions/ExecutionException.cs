namespace DataSharedLib.Exceptions;

public class ExecutionException : DatabaseException
{
    public ExecutionException(string message) : base(message) { }
    public ExecutionException(string message, Exception innerException) : base(message, innerException) { }
}
