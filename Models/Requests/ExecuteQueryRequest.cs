namespace DataSharedLib.Models.Requests;

public class ExecuteQueryRequest
{
    public string Sql { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    public int? CommandTimeoutSeconds { get; set; }
}
