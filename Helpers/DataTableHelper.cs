namespace DataSharedLib.Helpers;

using System.Data;
using System.Reflection;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(IEnumerable<T> items) where T : class
    {
        var dataTable = new DataTable(typeof(T).Name);
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dataTable.Columns.Add(prop.Name, propType);
        }

        foreach (var item in items)
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
