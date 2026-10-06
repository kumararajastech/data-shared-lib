# DataSharedLib

A modern, high-performance **.NET 10** SQL Server class library providing connection resilience, dynamic CRUD operations, reflection-optimized bulk inserts (`SqlBulkCopy`), and typed stored procedure handling.

---

## Features

- **Built for .NET 10 (`net10.0`)** using C# 13 standards and nullable reference types.
- **Connection Factory**: Resilient `SqlConnection` handling with configurable retry limits.
- **Dynamic CRUD Service**: Safe parameterized SQL generator with pagination support.
- **High-Performance Bulk Copy**: Streamlined `SqlBulkCopy` integration for batch inserts.
- **Stored Procedure Engine**: Dynamic input/output parameter mapping and multi-grid result reading.
- **Security First**: Guard clause input validation against SQL injection risks.
- **AI Agent Ready**: Includes `AGENTS.md` and complete project stubs for AI coding workflows.

---

## Quick Start

### 1. Register in Dependency Injection (`Program.cs`)

```csharp
using DataSharedLib.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Register from Configuration (appsettings.json section "DatabaseConnection")
builder.Services.AddDataSharedLib(builder.Configuration);

// Or register via delegate options
builder.Services.AddDataSharedLib(options =>
{
    options.ConnectionString = "Server=localhost;Database=AppDb;Trusted_Connection=True;TrustServerCertificate=True;";
    options.CommandTimeoutSeconds = 30;
    options.MaxRetryCount = 3;
});
```

### 2. Perform a Paginated Read

```csharp
public class CustomerService(IDatabaseCrudService crudService)
{
    public async Task FetchCustomersAsync()
    {
        var response = await crudService.ReadAsync(new ReadRequest
        {
            TableName = "Customers",
            SelectColumns = new[] { "CustomerId", "Name", "Email" },
            WhereClause = "[IsActive] = @IsActive",
            Parameters = new Dictionary<string, object?> { { "IsActive", true } },
            OrderBy = "[Name] ASC",
            PageNumber = 1,
            PageSize = 50
        });

        foreach (var row in response.Rows)
        {
            Console.WriteLine($"Customer: {row["Name"]} ({row["Email"]})");
        }
    }
}
```

### 3. Perform a Bulk Insert

```csharp
public async Task BulkInsertProductsAsync(IEnumerable<ProductModel> products, IDatabaseBulkService bulkService)
{
    var response = await bulkService.BulkInsertAsync(new BulkCreateRequest<ProductModel>
    {
        TableName = "Products",
        Items = products,
        BatchSize = 5000
    });

    Console.WriteLine($"Inserted {response.RowsAffected} rows in {response.ExecutionTimeMs} ms.");
}
```
