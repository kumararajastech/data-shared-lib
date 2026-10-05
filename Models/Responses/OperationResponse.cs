namespace DataSharedLib.Models.Responses;

public class OperationResponse
{
    public bool IsSuccess { get; set; }
    public int RowsAffected { get; set; }
    public object? PrimaryKeyId { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan ElapsedTime { get; set; }
}
