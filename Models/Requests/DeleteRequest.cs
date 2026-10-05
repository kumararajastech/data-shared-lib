namespace DataSharedLib.Models.Requests;

public class DeleteRequest
{
    public string TableName { get; set; } = string.Empty;
    public string WhereClause { get; set; } = string.Empty;
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
}
