namespace DataSharedLib.Models.Requests;

using System.Data;

public class StoredProcedureRequest
{
    public required string ProcedureName { get; set; }
    public Dictionary<string, object?>? Parameters { get; set; }
    public Dictionary<string, SqlDbType>? OutputParameters { get; set; }
    public int? TimeoutSeconds { get; set; }
}
