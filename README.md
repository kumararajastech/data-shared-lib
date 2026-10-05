# DataSharedLib (.NET 10 SQL Server Library)

A enterprise-ready .NET 10 C# class library designed for high-throughput, asynchronous SQL Server interactions.

## Architecture & Folders
- **Connection**: `IDatabaseConnectionFactory`, `DatabaseConnectionFactory`, `DatabaseConnectionOptions`
- **Services**: `IDatabaseQueryService`, `IDatabaseCrudService`, `IDatabaseBulkService`, `IStoredProcedureService`
- **Models**: Strongly-typed Requests & Responses
- **Validation**: Guard conditions protecting against invalid parameters and unsafe inputs
- **Exceptions**: Custom domain exceptions (`DatabaseException`, `ConnectionException`, `ExecutionException`)
- **Helpers**: Dynamic DataTable conversion (`DataTableHelper`) and `SqlParameter` mappers
- **Configuration**: Service collection extension methods for seamless DI setup

## Dependency Injection Setup
```csharp
using DataSharedLib.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Register with appsettings.json section "DatabaseConnection"
builder.Services.AddDataSharedLib(builder.Configuration);

// Or programmatically:
builder.Services.AddDataSharedLib(options =>
{
    options.ConnectionString = "Server=localhost;Database=AppDb;Trusted_Connection=True;TrustServerCertificate=True;";
    options.CommandTimeoutSeconds = 30;
});
```
