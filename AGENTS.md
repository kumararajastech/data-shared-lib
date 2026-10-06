# AGENTS.md - Coding Assistant & AI Agent Guidelines

## 1. Project Overview & Context
- **Project Name**: `DataSharedLib` (`data-shared-lib`)
- **Target Framework**: .NET 10 (`net10.0`)
- **Database Target**: Microsoft SQL Server / Azure SQL
- **Primary Package Dependencies**: `Microsoft.Data.SqlClient` (v5.2.2+), `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Options`

`DataSharedLib` is a foundational, enterprise-grade class library providing high-performance, resilient, and safe SQL Server database operations. It encapsulates connection handling, dynamic CRUD, raw query execution, bulk inserts (`SqlBulkCopy`), and stored procedure invocations with robust parameterization and strict guard validation against SQL injection.

---

## 2. Architectural Principles & Patterns
When generating, modifying, or extending code in this repository, agents **MUST** follow these core architectural rules:

1. **Separation of Concerns**:
   - `Connection`: Connection creation, retry logic, options validation (`IDatabaseConnectionFactory`).
   - `Services`: Core business/database logic abstractions (`IDatabaseQueryService`, `IDatabaseCrudService`, `IDatabaseBulkService`, `IStoredProcedureService`).
   - `Models`: Data contracts split into `Requests/` and `Responses/`.
   - `Validation`: Input guard validation (`IRequestValidator`).
   - `Exceptions`: Custom domain-specific exception hierarchy (`DatabaseException`, `ConnectionException`, `ExecutionException`).
   - `Helpers`: Internal utilities for `SqlParameter` generation and reflection-based `DataTable` conversions.
   - `Configuration`: ServiceCollection extension methods (`AddDataSharedLib`).

2. **Security & Parameterization First**:
   - **NEVER** use string concatenation or interpolation for user-supplied data in raw SQL statements.
   - All dynamic parameters **MUST** be passed using `Microsoft.Data.SqlClient.SqlParameter` instances or `Dictionary<string, object?>`.
   - Column names and table names MUST pass through `IRequestValidator.SanitizeIdentifier` or strict regex matching before inclusion in dynamic SQL statements.

3. **Asynchronous & Resilient Core**:
   - All I/O operations **MUST** provide asynchronous methods (`Async` suffix) returning `Task<T>` or `Task`.
   - Accept `CancellationToken` in all async service method signatures, defaulting to `default`.
   - Use `await using` for SQL connections, commands, and readers to guarantee proper unmanaged resource disposal.

4. **Exception Handling**:
   - Catch raw `SqlException` and wrap them in `ConnectionException` or `ExecutionException`.
   - Preserve original exception context via inner exceptions and extract relevant SQL Error numbers (`ex.Number`).

---

## 3. C# / .NET 10 Coding Standards
- **Language Version**: C# 13 / .NET 10 standard.
- **Namespaces**: Use file-scoped namespaces (`namespace DataSharedLib.Services;`).
- **Nullability**: Nullable reference types are enabled (`<Nullable>enable</Nullable>`). Explicitly mark nullable properties (`string?`).
- **Constructors**: Prefer C# primary constructors or standard constructor injection for service classes.
- **Formatting**: Adhere to standard Microsoft C# guidelines (`.editorconfig` is included in the project root).
- **Control Flow**: Use guard clauses at the top of methods to return early or throw validation exceptions (`ArgumentNullException.ThrowIfNull`).

---

## 4. Testing & Validation Checklist for Agents
Before proposing PRs or submitting code modifications:
- [ ] Ensure all public methods have XML documentation comments.
- [ ] Verify `IRequestValidator` checks are invoked at the entry point of service methods.
- [ ] Confirm no secrets, connection strings, or passwords are hardcoded in test files or defaults.
- [ ] Verify that new models or requests implement clean property initialization.
- [ ] Check that `.github/workflows/ci.yml` builds cleanly on `.NET 10`.

---

## 5. File Structure Reference
```
data-shared-lib
│
├── AGENTS.md                           # AI Agent & Assistant Guidelines
├── ARCHITECTURE.md                     # Deep-dive architecture design
├── CONTRIBUTING.md                     # Contributor workflow
├── CODE_OF_CONDUCT.md                  # Community behavior standards
├── .editorconfig                       # Formatting rules
├── .gitignore                          # Visual Studio & .NET ignore list
├── README.md                           # Quick start & library docs
├── DataSharedLib.csproj                # .NET 10 project definition
│
├── Connection/                         # Connection factories & options
├── Services/                           # Core database abstraction services
├── Models/
│   ├── Requests/                       # Standard request contracts
│   └── Responses/                      # Standard response contracts
├── Validation/                         # Security & input validators
├── Exceptions/                         # Custom domain exceptions
├── Helpers/                            # Reflection & SqlParameter helpers
├── Configuration/                      # DI ServiceCollection extensions
└── .github/                            # CI/CD workflows & issue templates
```
