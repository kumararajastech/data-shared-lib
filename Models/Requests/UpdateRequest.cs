namespace DataSharedLib.Models.Requests;

public class UpdateRequest
{
    public string TableName { get; set; } = string.Empty;
    public IDictionary<string, object?> ColumnValues { get; set; } = new Dictionary<string, object?>();
    public string WhereClause { get; set; } = string.Empty;
    public IDictionary<string, object?> WhereParameters { get; set; } = new Dictionary<string, object?>();
}
