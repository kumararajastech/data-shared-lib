# AI Agent Guidelines - DataSharedLib (.NET 10)

## Overview
This repository contains the `DataSharedLib` .NET 10 C# class library for SQL Server. All AI agents (Architect, Backend, DevOps, Data, Testing) must follow these strict rules.

## Core Rules
1. **Parameterized Queries Only**: Never use string concatenation for raw user values in SQL statements.
2. **Identifier Sanitization**: Validate table names and column names against strict regex (`^[a-zA-Z0-9_]+$`) via `IRequestValidator`.
3. **Asynchronous I/O**: Use `async`/`await` across all database methods, supporting `CancellationToken`.
4. **Resource Management**: Always dispose connections and commands via `await using`.
5. **Exception Handling**: Catch `SqlException` and wrap in domain-specific `DatabaseException`, `ConnectionException`, or `ExecutionException`.
