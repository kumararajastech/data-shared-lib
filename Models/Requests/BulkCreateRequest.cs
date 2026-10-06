namespace DataSharedLib.Models.Requests;

using System.Data;

public class BulkCreateRequest
{
    public required string TableName { get; set; }
    public required DataTable DataTable { get; set; }
    public Dictionary<string, string>? ColumnMappings { get; set; }
    public int BatchSize { get; set; } = 5000;
    public int? TimeoutSeconds { get; set; } = 60;
}

public class BulkCreateRequest<T> where T : class
{
    public required string TableName { get; set; }
    public required IEnumerable<T> Items { get; set; }
    public int BatchSize { get; set; } = 5000;
}
