namespace DataSharedLib.Models.Requests;

public class ReadRequest
{
    public required string TableName { get; set; }
    public IEnumerable<string>? SelectColumns { get; set; }
    public string? WhereClause { get; set; }
    public Dictionary<string, object?>? Parameters { get; set; }
    public string? OrderBy { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
