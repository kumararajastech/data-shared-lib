using System.Data;

namespace DataSharedLib.Models.Requests;

public class StoredProcedureRequest
{
    public string ProcedureName { get; set; } = string.Empty;
    public IDictionary<string, object?> InputParameters { get; set; } = new Dictionary<string, object?>();
    public IDictionary<string, SqlDbType> OutputParameters { get; set; } = new Dictionary<string, SqlDbType>();
    public int? TimeoutSeconds { get; set; }
}
