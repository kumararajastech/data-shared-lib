namespace DataSharedLib.Models.Responses;

public class StoredProcedureResponse
{
    public bool Success { get; set; }
    public IReadOnlyList<IReadOnlyList<IReadOnlyDictionary<string, object?>>> ResultSets { get; set; } = Array.Empty<IReadOnlyList<IReadOnlyDictionary<string, object?>>>();
    public IReadOnlyDictionary<string, object?> OutputParameters { get; set; } = new Dictionary<string, object?>();
    public int ReturnValue { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
