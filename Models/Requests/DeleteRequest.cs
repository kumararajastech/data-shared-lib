namespace DataSharedLib.Models.Requests;

public class DeleteRequest
{
    public string TableName { get; set; } = string.Empty;
    public string WhereClause { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
}
