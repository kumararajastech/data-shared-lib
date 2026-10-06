namespace DataSharedLib.Models.Responses;

public class OperationResponse
{
    public bool Success { get; set; }
    public int RowsAffected { get; set; }
    public object? GeneratedIdentifier { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
