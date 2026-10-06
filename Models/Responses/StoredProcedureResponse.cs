namespace DataSharedLib.Models.Responses;

public class StoredProcedureResponse
{
    public bool Success { get; set; }
    public int ReturnCode { get; set; }
    public Dictionary<string, object?> OutputValues { get; set; } = [];
    public IEnumerable<IEnumerable<Dictionary<string, object?>>> ResultGrids { get; set; } = [];
    public long ExecutionTimeMs { get; set; }
}
