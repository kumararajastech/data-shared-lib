using DataSharedLib.Models.Requests;

namespace DataSharedLib.Validation;

public class RequestValidator : IRequestValidator
{
    public void ValidateExecuteQuery(ExecuteQueryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.CommandText))
            throw new ArgumentException("CommandText cannot be empty.", nameof(request));
    }

    public void ValidateRead(ReadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.TableName))
            throw new ArgumentException("TableName cannot be empty.", nameof(request));
    }

    public void ValidateCreate(CreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.TableName))
            throw new ArgumentException("TableName cannot be empty.", nameof(request));
        if (request.ColumnValues == null || request.ColumnValues.Count == 0)
            throw new ArgumentException("ColumnValues must contain at least one field.", nameof(request));
    }

    public void ValidateBulkCreate(BulkCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.DestinationTableName))
            throw new ArgumentException("DestinationTableName cannot be empty.", nameof(request));
        if (request.DataTable == null)
            throw new ArgumentException("DataTable cannot be null.", nameof(request));
    }

    public void ValidateUpdate(UpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.TableName))
            throw new ArgumentException("TableName cannot be empty.", nameof(request));
        if (request.ColumnValues == null || request.ColumnValues.Count == 0)
            throw new ArgumentException("ColumnValues must contain at least one field.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.WhereClause))
            throw new ArgumentException("WhereClause is required for Update operations to prevent accidental table-wide updates.", nameof(request));
    }

    public void ValidateDelete(DeleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.TableName))
            throw new ArgumentException("TableName cannot be empty.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.WhereClause))
            throw new ArgumentException("WhereClause is required for Delete operations to prevent accidental table-wide deletes.", nameof(request));
    }

    public void ValidateStoredProcedure(StoredProcedureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.ProcedureName))
            throw new ArgumentException("ProcedureName cannot be empty.", nameof(request));
    }
}
