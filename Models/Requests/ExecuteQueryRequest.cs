namespace DataSharedLib.Models.Requests;

public class ExecuteQueryRequest
{
    public string CommandText { get; set; } = string.Empty;
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    public int? TimeoutSeconds { get; set; }
}
