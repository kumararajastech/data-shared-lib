namespace DataSharedLib.Models.Requests;

public class ReadRequest
{
    public string TableName { get; set; } = string.Empty;
    public IReadOnlyList<string>? SelectColumns { get; set; }
    public string? WhereClause { get; set; }
    public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    public string? OrderBy { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
