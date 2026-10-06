namespace DataSharedLib.Models.Requests;

public class UpdateRequest
{
    public required string TableName { get; set; }
    public required Dictionary<string, object?> ColumnValues { get; set; }
    public required string WhereClause { get; set; }
    public Dictionary<string, object?>? Parameters { get; set; }
}
