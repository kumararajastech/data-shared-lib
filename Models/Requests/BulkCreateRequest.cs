namespace DataSharedLib.Models.Requests;

using System.Data;

public class BulkCreateRequest
{
    public string TableName { get; set; } = string.Empty;
    public DataTable Data { get; set; } = new DataTable();
    public int BatchSize { get; set; } = 5000;
    public int TimeoutSeconds { get; set; } = 300;
    public IReadOnlyDictionary<string, string>? ColumnMappings { get; set; }
}
