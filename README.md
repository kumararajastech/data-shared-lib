# DataSharedLib (.NET 10 C# SQL Server Library)

High-performance, secure, and resilient SQL Server database abstraction library built for .NET 10.

## Features
- **Resilient Connections**: Connection factory with configurable timeouts and retries.
- **Parameterized Query Service**: Strongly-typed and dynamic SQL query execution.
- **Dynamic CRUD Service**: Guarded CRUD operations with OFFSET/FETCH pagination.
- **Bulk Insert Service**: High-speed batch streaming using `SqlBulkCopy`.
- **Stored Procedure Service**: Output parameter and multi-result set handling.
- **AI Agent Memory & Structure**: Complete `.github/` suite with Copilot instructions, skills, chatmodes, agents, and prompts matching the repository memory layout.

## Quick Start (Dependency Injection)
```csharp
using DataSharedLib.Configuration;

builder.Services.AddDataSharedLib(builder.Configuration);
```
