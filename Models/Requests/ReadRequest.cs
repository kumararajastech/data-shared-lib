namespace DataSharedLib.Models.Requests;

public class ReadRequest
{
    public string TableName { get; set; } = string.Empty;
    public IEnumerable<string>? SelectColumns { get; set; }
    public string? WhereClause { get; set; }
    public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
    public string? OrderBy { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
