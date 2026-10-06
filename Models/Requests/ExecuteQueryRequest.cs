namespace DataSharedLib.Models.Requests;

public class ExecuteQueryRequest
{
    public required string SqlText { get; set; }
    public Dictionary<string, object?>? Parameters { get; set; }
    public int? TimeoutSeconds { get; set; }
}
