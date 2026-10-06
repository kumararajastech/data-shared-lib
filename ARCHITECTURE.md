# Architecture Blueprint - DataSharedLib

## Layer Breakdown
1. **Connection Layer**: `IDatabaseConnectionFactory` creates authenticated `SqlConnection` instances with retry policies.
2. **Service Layer**: `IDatabaseQueryService`, `IDatabaseCrudService`, `IDatabaseBulkService`, `IStoredProcedureService`.
3. **Models**: Strictly typed request and response contracts.
4. **Validation Layer**: `IRequestValidator` guards input models against SQL injection and missing parameters.
5. **Configuration Layer**: Extension methods for `IServiceCollection` registration.
