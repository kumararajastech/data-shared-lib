namespace DataSharedLib.Models.Responses;

public class QueryResponse
{
    public IEnumerable<Dictionary<string, object?>> Rows { get; set; } = [];
    public int TotalCount { get; set; }
    public long ExecutionTimeMs { get; set; }
}
