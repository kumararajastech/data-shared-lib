namespace DataSharedLib.Helpers;

using Microsoft.Data.SqlClient;

public static class SqlParameterHelper
{
    public static SqlParameter[] ToSqlParameters(IReadOnlyDictionary<string, object?> parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return Array.Empty<SqlParameter>();

        return parameters.Select(kvp =>
        {
            var paramName = kvp.Key.StartsWith("@") ? kvp.Key : $"@{kvp.Key}";
            return new SqlParameter(paramName, kvp.Value ?? DBNull.Value);
        }).ToArray();
    }
}
