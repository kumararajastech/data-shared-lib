namespace DataSharedLib.Services;

using System.Data;
using Microsoft.Data.SqlClient;

using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;

public class StoredProcedureService : IStoredProcedureService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IRequestValidator _validator;

    public StoredProcedureService(IDatabaseConnectionFactory connectionFactory, IRequestValidator validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<StoredProcedureResponse> ExecuteStoredProcedureAsync(StoredProcedureRequest request, CancellationToken cancellationToken = default)
    {
        _validator.ValidateStoredProcedure(request);

        var startTime = DateTime.UtcNow;
        var sanitizedSpName = _validator.SanitizeIdentifier(request.ProcedureName);

        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        command.CommandText = sanitizedSpName;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandTimeout = request.TimeoutSeconds;

        if (request.Parameters != null)
        {
            command.Parameters.AddRange(SqlParameterHelper.ToSqlParameters(request.Parameters));
        }

        var returnParam = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
        returnParam.Direction = ParameterDirection.ReturnValue;

        try
        {
            var resultSets = new List<IReadOnlyList<IReadOnlyDictionary<string, object?>>>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            do
            {
                var grid = new List<IReadOnlyDictionary<string, object?>>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    grid.Add(row);
                }
                resultSets.Add(grid);
            } while (await reader.NextResultAsync(cancellationToken));

            var outputParams = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (SqlParameter p in command.Parameters)
            {
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                {
                    outputParams[p.ParameterName.TrimStart('@')] = p.Value == DBNull.Value ? null : p.Value;
                }
            }

            int returnVal = returnParam.Value is int iVal ? iVal : 0;

            return new StoredProcedureResponse
            {
                Success = true,
                ResultSets = resultSets,
                OutputParameters = outputParams,
                ReturnValue = returnVal,
                ExecutionTime = DateTime.UtcNow - startTime
            };
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Stored procedure execution failed: {ex.Message}", sanitizedSpName, ex.Number, ex);
        }
    }
}
