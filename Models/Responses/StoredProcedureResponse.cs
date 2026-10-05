using System.Data;

namespace DataSharedLib.Models.Responses;

public class StoredProcedureResponse
{
    public bool IsSuccess { get; set; }
    public int ReturnCode { get; set; }
    public IDictionary<string, object?> OutputValues { get; set; } = new Dictionary<string, object?>();
    public DataSet ResultSets { get; set; } = new DataSet();
    public TimeSpan ElapsedTime { get; set; }
}
