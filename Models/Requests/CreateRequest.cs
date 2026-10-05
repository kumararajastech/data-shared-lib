namespace DataSharedLib.Models.Requests;

public class CreateRequest
{
    public string TableName { get; set; } = string.Empty;
    public IDictionary<string, object?> ColumnValues { get; set; } = new Dictionary<string, object?>();
    public bool ReturnIdentity { get; set; } = true;
}
