namespace DataSharedLib.Models.Responses;

public class QueryResponse<T>
{
    public bool Success { get; set; }
    public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();
    public int TotalRecords { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
