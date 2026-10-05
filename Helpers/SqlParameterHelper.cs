using System.Data;
using Microsoft.Data.SqlClient;

namespace DataSharedLib.Helpers;

public static class SqlParameterHelper
{
    public static SqlParameter[] CreateParameters(IDictionary<string, object?>? parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return Array.Empty<SqlParameter>();
        }

        var sqlParams = new List<SqlParameter>(parameters.Count);
        foreach (var kvp in parameters)
        {
            var paramName = kvp.Key.StartsWith("@") ? kvp.Key : "@" + kvp.Key;
            var paramValue = kvp.Value ?? DBNull.Value;
            sqlParams.Add(new SqlParameter(paramName, paramValue));
        }

        return sqlParams.ToArray();
    }
}
