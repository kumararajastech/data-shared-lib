using System.Data;
using System.Diagnostics;
using DataSharedLib.Connection;
using DataSharedLib.Exceptions;
using DataSharedLib.Helpers;
using DataSharedLib.Models.Requests;
using DataSharedLib.Models.Responses;
using DataSharedLib.Validation;
using Microsoft.Data.SqlClient;

namespace DataSharedLib.Services;

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
        var sw = Stopwatch.StartNew();

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = request.ProcedureName;
        if (request.TimeoutSeconds.HasValue) command.CommandTimeout = request.TimeoutSeconds.Value;

        command.Parameters.AddRange(SqlParameterHelper.CreateParameters(request.InputParameters));

        var returnParam = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
        returnParam.Direction = ParameterDirection.ReturnValue;

        var outSqlParams = new Dictionary<string, SqlParameter>();
        if (request.OutputParameters != null)
        {
            foreach (var kvp in request.OutputParameters)
            {
                var paramName = kvp.Key.StartsWith("@") ? kvp.Key : "@" + kvp.Key;
                var param = command.Parameters.Add(paramName, kvp.Value);
                param.Direction = ParameterDirection.Output;
                outSqlParams[kvp.Key] = param;
            }
        }

        var dataSet = new DataSet();
        try
        {
            using var adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);
        }
        catch (SqlException ex)
        {
            throw new ExecutionException($"Stored procedure execution failed: {ex.Message}", request.ProcedureName, ex, ex.Number);
        }

        var outputValues = new Dictionary<string, object?>();
        foreach (var kvp in outSqlParams)
        {
            outputValues[kvp.Key] = kvp.Value.Value == DBNull.Value ? null : kvp.Value.Value;
        }

        int returnCode = returnParam.Value is int rc ? rc : 0;

        sw.Stop();
        return new StoredProcedureResponse
        {
            IsSuccess = true,
            ReturnCode = returnCode,
            OutputValues = outputValues,
            ResultSets = dataSet,
            ElapsedTime = sw.Elapsed
        };
    }
}
