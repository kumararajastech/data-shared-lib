namespace DataSharedLib.Models.Responses;

public class OperationResponse
{
    public bool Success { get; set; }
    public int RowsAffected { get; set; }
    public object? GeneratedPrimaryKey { get; set; }
    public long ExecutionTimeMs { get; set; }
}
