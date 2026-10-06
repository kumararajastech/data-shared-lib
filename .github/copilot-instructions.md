# GitHub Copilot Workspace Instructions - DataSharedLib

This repository contains `DataSharedLib`, a .NET 10 / C# 13 database abstraction library for SQL Server.

## Standards & Constraints
- Target Framework: `net10.0`
- Language Version: `13.0`
- Driver: `Microsoft.Data.SqlClient`
- All SQL parameters must use `SqlParameter` or dictionary parameterization. String concatenation is strictly prohibited.
- Identifiers must be sanitized via `IRequestValidator.SanitizeIdentifier()`.
- Async/await required for all I/O, supporting `CancellationToken`.
- Resource cleanup via `await using`.
