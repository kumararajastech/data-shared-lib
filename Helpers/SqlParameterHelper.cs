namespace DataSharedLib.Helpers;

using Microsoft.Data.SqlClient;

public static class SqlParameterHelper
{
    public static SqlParameter[] ToSqlParameters(Dictionary<string, object?> parameters)
    {
        if (parameters == null || parameters.Count == 0) return Array.Empty<SqlParameter>();

        var result = new List<SqlParameter>(parameters.Count);
        foreach (var kvp in parameters)
        {
            var paramName = kvp.Key.StartsWith("@") ? kvp.Key : "@" + kvp.Key;
            result.Add(new SqlParameter(paramName, kvp.Value ?? DBNull.Value));
        }
        return result.ToArray();
    }
}
