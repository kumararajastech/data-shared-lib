namespace DataSharedLib.Models.Responses;

public class QueryResponse<T>
{
    public IEnumerable<T> Records { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public TimeSpan ElapsedTime { get; set; }
}
