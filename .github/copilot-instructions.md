# JobTrackerVSA AI Coding Agent Guidelines

This file provides the context and architectural boundaries necessary to write correct, idiomatic code within JobTrackerVSA.

## 🏗 Architecture & Structural Patterns
- **Vertical Slice Architecture**: Code is organized strictly by feature. Do not extract to artificial layers (e.g., Application/Services folders). Navigation starts at `Features/{FeatureName}/{Action}/` (e.g., `Features/JobApplications/Add/`).
- **Feature Structure**: Each folder encapsulates an action. It typically contains a Razor Page (`Add.cshtml`), its PageModel (`Add.cshtml.cs`), a MediatR Request (`AddJobApplicationCommand.cs`), and its Handler (`AddJobApplicationHandler.cs`).
- **CQRS via MediatR**:
  - **Commands** (writes) and **Queries** (reads) implement `IRequest<Result<T>>` or `IRequest<Result>`.
  - **Result Pattern**: Never throw exceptions for business rules. Use `Result.Failure("...")` or `Result<T>.Success(data)` from `JobTrackerVSA.Web.Infrastructure.Shared`. Unhandled system exceptions are intercepted centrally via `UnhandledExceptionBehavior`.
  - **Dependency Injection**: Use **C# 12 Primary Constructors** on Handlers to inject dependencies (e.g., `public class MyHandler(AppDbContext db) : IRequestHandler...`).
- **Validation**: Place `[Required]`, `[StringLength]`, and custom validation attributes like `[ResumeValidation]` directly on Command/Query records.

## 💾 Data & State Management
- **Entity Framework Core**: The `AppDbContext` (`Data/AppDbContext.cs`) applies Global Query Filters for user isolation. Never circumvent this isolation.
- **Interceptors**: State change side-effects (like syncing an application status when an interview is added) are handled in `Data/Interceptors/` (e.g., `InterviewStatusInterceptor.cs`).
- **Auth & Document Storage**: Inject `ICurrentUserService` (`Infrastructure/Auth/`) to get the current user ID, and `IResumeStorageService` (`Infrastructure/Storage/`) for Azure Blob interactions (Azurite during dev).
  - **Architecture**: Resumes/CVs are saved securely in Azure Blob Storage using a private-by-default container (`publicAccess: 'None'`).
  - **Upload Validation**: File uploads enforce strict server-side validation (e.g., Magic Numbers checking) to prevent malicious files.
  - **Viewing Documents**: Direct public access to the blob URLs is forbidden. To view a document, the system generates a secure, short-lived SAS (Shared Access Signature) URL and redirects the user to it (e.g., handled in `ViewResumeEndpoint.cs` via `/job-applications/{id:guid}/resume`).

## 🛠 Critical Developer Workflows
- **Migrations**: Always specify the web project when creating and applying migrations from the root folder:
  ```powershell
  dotnet ef migrations add <MigrationName> --project JobTrackerVSA/JobTrackerVSA.Web.csproj --startup-project JobTrackerVSA/JobTrackerVSA.Web.csproj
  dotnet ef database update --project JobTrackerVSA/JobTrackerVSA.Web.csproj --startup-project JobTrackerVSA/JobTrackerVSA.Web.csproj
  ```
- **Testing**: Run all unit tests by executing `dotnet test` within the `tests/JobTrackerVSA.UnitTests` folder. Tests utilize xUnit, FluentAssertions, NSubstitute, and an EF Core In-Memory database paired with `TestDbContextFactory` to mock authentication states.

## ⚠️ Project-Specific Conventions
- **Language**: English is mandatory for all code, documentation, and internal comments.
- **Modern C# Tooling**: Prioritize modern .NET 8/10 features. Favor `collection expressions []`, `target-typed new()`, and `primary constructors`.
- **SOLID Principles**: Apply SOLID principles to all modifications. Ensure classes have a single responsibility, are open for extension but closed for modification, follow Liskov substitution, keep interfaces segregated, and depend on abstractions where appropriate.
- **Commits**: Never auto-commit or stage files without explicit user permission. If asked to write a commit message, YOU MUST explicitly run `git diff` AND `git diff --staged` to inspect the exact changes before generating the message. **DO NOT HALLUCINATE OR RELY ON MEMORY.** You must generate the commit message STRICTLY based on the terminal output of the diff. Do not assume any changes based on the conversation history, as the user may have already committed, modified, or discarded files manually. Use conventional commits (`feat: `, `fix: `).

## ✅ Definition of Done (DoD)
Before making a commit, or when asked to generate a commit message, YOU MUST review and verify the following Definition of Done:
- **Unit Tests**: Create or update unit tests if applicable. **CRITICAL: You MUST run `dotnet test` in the terminal inside the `tests/JobTrackerVSA.UnitTests` folder and ensure all tests pass before considering the feature done or generating a commit message.** Do not assume tests pass without running them.
- **Documentation**: Update the `README.md` (or other relevant documentation) if applicable.
- **Seed Data**: If a new feature or entity property is added, ensure it is included in the dummy data generation within `Features/Maintenance/MaintenanceEndpoints.cs`.