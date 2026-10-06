namespace DataSharedLib.Models.Requests;

public class DeleteRequest
{
    public required string TableName { get; set; }
    public required string WhereClause { get; set; }
    public Dictionary<string, object?>? Parameters { get; set; }
}
