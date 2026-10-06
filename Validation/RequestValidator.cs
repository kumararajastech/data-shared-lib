namespace DataSharedLib.Validation;

using System.Text.RegularExpressions;
using DataSharedLib.Models.Requests;

public class RequestValidator : IRequestValidator
{
    private static readonly Regex IdentifierRegex = new(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    public string SanitizeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty.");

        var clean = identifier.Trim().Trim('[', ']');
        if (!IdentifierRegex.IsMatch(clean))
            throw new ArgumentException($"Invalid database identifier: '{identifier}'.");

        return $"[{clean}]";
    }

    public void ValidateRead(ReadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SanitizeIdentifier(request.TableName);
    }

    public void ValidateCreate(CreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SanitizeIdentifier(request.TableName);
        if (request.ColumnValues == null || request.ColumnValues.Count == 0)
            throw new ArgumentException("ColumnValues cannot be empty for Create operation.");
    }

    public void ValidateUpdate(UpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SanitizeIdentifier(request.TableName);
        if (request.ColumnValues == null || request.ColumnValues.Count == 0)
            throw new ArgumentException("ColumnValues cannot be empty for Update operation.");
        if (string.IsNullOrWhiteSpace(request.WhereClause))
            throw new ArgumentException("WhereClause is required for Update operation to prevent accidental full table update.");
    }

    public void ValidateDelete(DeleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SanitizeIdentifier(request.TableName);
        if (string.IsNullOrWhiteSpace(request.WhereClause))
            throw new ArgumentException("WhereClause is required for Delete operation to prevent accidental full table delete.");
    }

    public void ValidateBulkCreate(BulkCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SanitizeIdentifier(request.TableName);
        if (request.Data == null || request.Data.Rows.Count == 0)
            throw new ArgumentException("DataTable cannot be null or empty for BulkCreate operation.");
    }

    public void ValidateExecuteQuery(ExecuteQueryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Sql))
            throw new ArgumentException("SQL text cannot be empty.");
    }

    public void ValidateStoredProcedure(StoredProcedureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SanitizeIdentifier(request.ProcedureName);
    }
}
