namespace DataSharedLib.Validation;

using System.Text.RegularExpressions;
using DataSharedLib.Exceptions;
using DataSharedLib.Models.Requests;

public class RequestValidator : IRequestValidator
{
    private static readonly Regex IdentifierRegex = new(@"^[a-zA-Z0-9_\[\]\.]+$", RegexOptions.Compiled);

    public void ValidateReadRequest(ReadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateTableIdentifier(request.TableName);
    }

    public void ValidateCreateRequest(CreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateTableIdentifier(request.TableName);
        if (request.ColumnValues == null || !request.ColumnValues.Any())
        {
            throw new DatabaseException("CreateRequest must contain at least one column value.");
        }
    }

    public void ValidateUpdateRequest(UpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateTableIdentifier(request.TableName);
        if (request.ColumnValues == null || !request.ColumnValues.Any())
        {
            throw new DatabaseException("UpdateRequest must contain at least one column value to update.");
        }
        if (string.IsNullOrWhiteSpace(request.WhereClause))
        {
            throw new DatabaseException("UpdateRequest must specify a WhereClause to prevent unconstrained updates.");
        }
    }

    public void ValidateDeleteRequest(DeleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateTableIdentifier(request.TableName);
        if (string.IsNullOrWhiteSpace(request.WhereClause))
        {
            throw new DatabaseException("DeleteRequest must specify a WhereClause to prevent unconstrained deletions.");
        }
    }

    public void ValidateExecuteQueryRequest(ExecuteQueryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.SqlText))
        {
            throw new DatabaseException("ExecuteQueryRequest must contain non-empty SqlText.");
        }
    }

    public void ValidateBulkCreateRequest(BulkCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateTableIdentifier(request.TableName);
        ArgumentNullException.ThrowIfNull(request.DataTable);
    }

    public void ValidateStoredProcedureRequest(StoredProcedureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateTableIdentifier(request.ProcedureName);
    }

    public string SanitizeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return string.Empty;
        var trimmed = identifier.Trim();
        if (!IdentifierRegex.IsMatch(trimmed))
        {
            throw new DatabaseException($"Invalid database identifier name: '{identifier}'");
        }
        return trimmed;
    }

    private void ValidateTableIdentifier(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new DatabaseException("Table or procedure name cannot be null or empty.");
        }
        SanitizeIdentifier(tableName);
    }
}
