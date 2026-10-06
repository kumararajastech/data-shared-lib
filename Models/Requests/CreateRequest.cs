namespace DataSharedLib.Models.Requests;

public class CreateRequest
{
    public required string TableName { get; set; }
    public required Dictionary<string, object?> ColumnValues { get; set; }
    public bool ReturnIdentity { get; set; } = true;
}
