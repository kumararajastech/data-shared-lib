using System.Data;
using System.Reflection;

namespace DataSharedLib.Helpers;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(IEnumerable<T> items, string? tableName = null)
    {
        var dataTable = new DataTable(tableName ?? typeof(T).Name);
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in properties)
        {
            Type propType = prop.PropertyType;
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propType = Nullable.GetUnderlyingType(propType)!;
            }
            dataTable.Columns.Add(prop.Name, propType);
        }

        foreach (T item in items)
        {
            var values = new object?[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i].GetValue(item, null) ?? DBNull.Value;
            }
            dataTable.Rows.Add(values);
        }

        return dataTable;
    }
}
