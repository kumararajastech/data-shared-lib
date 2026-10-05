using System.Data;

namespace DataSharedLib.Models.Requests;

public class BulkCreateRequest
{
    public string DestinationTableName { get; set; } = string.Empty;
    public DataTable DataTable { get; set; } = new DataTable();
    public int BatchSize { get; set; } = 5000;
    public int TimeoutSeconds { get; set; } = 300;
    public IDictionary<string, string>? ColumnMappings { get; set; }
}
