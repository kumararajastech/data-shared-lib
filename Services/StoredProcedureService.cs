namespace DataSharedLib.Services;

using System.Data;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

public class StoredProcedureService : IStoredProcedureService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public StoredProcedureService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<StoredProcedureResponse> ExecuteAsync(StoredProcedureRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateStoredProcedureRequest(request);
        var watch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await using var connection = await _connectionFactory.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = _validator.SanitizeIdentifier(request.ProcedureName);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = request.TimeoutSeconds ?? 30;

            if (request.Parameters != null)
            {
                command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
            }

            var outParams = new List<SqlParameter>();
            if (request.OutputParameters != null)
            {
                foreach (var kvp in request.OutputParameters)
                {
                    var p = new SqlParameter(kvp.Key, kvp.Value) { Direction = ParameterDirection.Output };
                    outParams.Add(p);
                    command.Parameters.Add(p);
                }
            }

            var returnParam = command.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int)
            {
                Direction = ParameterDirection.ReturnValue
            });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            var resultTables = new List<List<Dictionary<string, object?>>>();

            do
            {
                var tableRows = new List<Dictionary<string, object?>>();
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var val = reader.GetValue(i);
                        row[reader.GetName(i)] = val == DBNull.Value ? null : val;
                    }
                    tableRows.Add(row);
                }
                resultTables.Add(tableRows);
            } while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false));

            var outputValues = outParams.ToDictionary(p => p.ParameterName, p => p.Value == DBNull.Value ? null : p.Value);
            int returnCode = returnParam.Value != DBNull.Value ? (int)returnParam.Value : 0;

            watch.Stop();
            return new StoredProcedureResponse
            {
                Success = true,
                ReturnCode = returnCode,
                OutputValues = outputValues,
                ResultGrids = resultTables,
                ExecutionTimeMs = watch.ElapsedMilliseconds
            };
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            throw new ExecutionException($"Stored procedure execution failed: {ex.Message}", ex);
        }
    }
}
