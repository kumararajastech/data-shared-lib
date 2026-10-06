# Architecture Documentation - DataSharedLib

## Overview
`DataSharedLib` is a modular, high-performance C# class library built for **.NET 10** targeting **SQL Server** database workloads. It provides a clean abstraction layer over `Microsoft.Data.SqlClient` with enterprise features including connection resilience, dynamic CRUD operations, reflection-optimized bulk inserts via `SqlBulkCopy`, and typed stored procedure execution.

---

## Core Components

### 1. Connection Management (`Connection/`)
- **`DatabaseConnectionOptions`**: Holds connection strings, command timeouts, retry counts, and option validation logic.
- **`IDatabaseConnectionFactory`**: Responsible for creating and opening `SqlConnection` instances with optional retry handling for transient connection faults.

### 2. Services Abstraction (`Services/`)
- **`DatabaseQueryService`**: Handles arbitrary parameterized SQL execution, returning scalar results, dynamic data tables, or strongly-typed object mappings.
- **`DatabaseCrudService`**: Provides standard Create, Read, Update, Delete abstractions using safe parameterized SQL generator routines with built-in OFFSET/FETCH pagination.
- **`DatabaseBulkService`**: Wraps `SqlBulkCopy` to perform rapid batch writes using `DataTable` or strongly-typed `IEnumerable<T>` data models.
- **`StoredProcedureService`**: Manages stored procedure execution, handling input, output, and return-value SQL parameters alongside multiple result set grids.

### 3. Request & Response Contracts (`Models/`)
All database operations accept typed requests and return uniform response models:
- **`QueryResponse<T>`**: Wraps returned rows, row count, execution duration, and metadata.
- **`OperationResponse`**: Provides rows affected count, generated primary keys (`SCOPE_IDENTITY()`), and status indicators.
- **`StoredProcedureResponse`**: Exposes output parameter value dictionaries, return codes, and result tables.

### 4. Guard Validation & Security (`Validation/`)
- **`RequestValidator`**: Validates table names, column lists, and clause inputs against SQL injection patterns prior to query construction.

---

## Sequence Flow: Dynamic Read Request

```
[ Caller / App ] ──( ReadRequest )──> [ DatabaseCrudService ]
                                            │
                                            ├──> [ RequestValidator ] (Sanitizes & Validates)
                                            ├──> [ ConnectionFactory ] (Create & Open SqlConnection)
                                            ├──> [ SqlCommand ] (Build Parameterized SQL)
                                            └──> [ QueryResponse ] <── Return mapped results
```
