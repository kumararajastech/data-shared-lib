namespace DataSharedLib.Models.Requests;

public class StoredProcedureRequest
{
    public string ProcedureName { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}
