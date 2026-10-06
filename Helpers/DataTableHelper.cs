namespace DataSharedLib.Helpers;

using System.Data;
using System.Reflection;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(IEnumerable<T> items)
    {
        var table = new DataTable(typeof(T).Name);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanRead)
                             .ToArray();

        foreach (var prop in props)
        {
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            table.Columns.Add(prop.Name, propType);
        }

        foreach (var item in items)
        {
            var values = new object?[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null) ?? DBNull.Value;
            }
            table.Rows.Add(values);
        }

        return table;
    }
}
