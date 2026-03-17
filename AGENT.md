# Gemini Project Helper

This file provides context about the JobTrackerVSA project. It outlines the project's architecture, technology stack, and conventions to ensure that AI-driven development aligns with the established standards.

## 1. Project Overview
**JobTrackerVSA** is a vertical slice architecture application for tracking job applications and interviews.

### Tech Stack
- **Framework:** .NET 10 (ASP.NET Core Razor Pages)
- **Database:** MS SQL Server LocalDB (`(localdb)\mssqllocaldb`)
- **ORM:** Entity Framework Core (SQL Server)
- **Authentication:** Auth0 (OpenID Connect)
- **Observability:** Application Insights (Telemetry & Logging)
- **Architecture:** Vertical Slice Architecture (Features folder), CQRS pattern with MediatR.

## 2. Architecture & Design Patterns

### Vertical Slice Architecture
The application is organized by **Features** rather than technical layers.
- **Path:** `JobTrackerVSA/Features/{FeatureName}/{Action}`
- **Example:** `Features/JobApplications/Add/` contains the Page, Model, Command, and Handler for adding a job application.

### CQRS (Command Query Responsibility Segregation)
- **Commands:** Handle writes/updates (e.g., `AddJobApplicationCommand`). Return `Result<T>` or `Result`.
- **Queries:** Handle reads (e.g., `GetJobApplicationsQuery`).
- **Mediator:** Used to decouple Pages from Handlers.

### Logging & Error Handling
- **Centralized Logging:** Handled via `Infrastructure/MediatR/UnhandledExceptionBehavior.cs`.
- **Telemetry:** Application Insights is configured for automatic exception tracking and request monitoring.


## 3. Key File Locations
- **DbContext:** `Data/AppDbContext.cs` (Includes Global Query Filters for Security).
- **Auth Service:** `Infrastructure/Auth/CurrentUserService.cs`
- **Unit Tests:** `tests/JobTrackerVSA.UnitTests/`
- **JS Utilities:** `wwwroot/js/site.js`
- **Infrastructure as Code (IaC):** `infrastructure/main.bicep`

## Storage: Azure Blob Storage (runtime context)
- **Path:** `JobTrackerVSA/Infrastructure/Storage/AzureBlobResumeStorageService.cs`
- **Purpose:** Securely manages the upload and deletion of job application resumes.
- **Security:** Ensures strict file validation through `ResumeValidationAttribute.cs` (checking size, extension, and Magic Numbers to prevent spoofing). Files are uploaded to an Azure Blob Container that must be configured with `PublicAccessType.None` to prevent unauthorized public access. In development, it relies on Azurite (`UseDevelopmentStorage=true`).

## Interceptor: InterviewStatusInterceptor (runtime context)
- **Path:** `JobTrackerVSA/Data/Interceptors/InterviewStatusInterceptor.cs`
- **Purpose:** Automatically updates a `JobApplication`'s `Status` when an `Interview` is added or modified. The interceptor runs inside the `SaveChanges` pipeline so the status change is part of the same transaction.
- **Mapping:** `InterviewType.Technical` → `ApplicationStatus.TechnicalTest`; `InterviewType.Proposal` → `ApplicationStatus.Offered`; otherwise → `ApplicationStatus.Interviewing`.
- **Registration:** Registered as a singleton and attached to the `DbContext` via `AddInterceptors(...)` in `Program.cs` so it executes on every `SaveChanges`.
- **Tests & Test infra:** Covered by `tests/JobTrackerVSA.UnitTests/Features/Interviews/InterviewStatusInterceptorTests.cs`. `TestDbContextFactory` wires the interceptor into in-memory contexts so tests observe the same runtime behavior.
- **Notes for future work:** Prefer loading tracked entities from `ChangeTracker.Local` to avoid unnecessary roundtrips; if an entity isn't tracked the interceptor falls back to a `Find` by PK. Keep behavior minimal and well-tested to avoid surprising side effects during saves.

## 4. Unit Testing
- **Frameworks:** xUnit, FluentAssertions, NSubstitute.
- **Strategy:** Uses EF Core In-Memory and `TestDbContextFactory` to mock `ICurrentUserService` and provide clean databases per test.
- **Pipeline:** Tests run automatically on every push to `main` via GitHub Actions.

## 5. Workflow Guidelines

### Git Workflow
-- **DO NOT** arbitrarily stage or commit files on your own.
-- **DO NOT** run `git add` or stage files. The user retains full control over staging;
	you must never stage changes yourself.
-- You may create commits only when the user explicitly requests it. Before committing,
	always present a `git diff` for review: if there are staged changes, show
	`git diff --staged`; if nothing is staged, show `git diff` of the working tree.
	After showing the diff, **do not** run `git add` to stage files—ask the user to
	stage changes or confirm how to proceed. Only commit what is already staged.
-- When asked to draft a commit message, you **MUST FIRST** run the appropriate
	`git status`/`git diff` command as described above to analyse the pending changes.
-- Commit messages must be in **English** and follow the conventional commits style
	(e.g., `feat: ...`, `fix: ...`). Check `git log` to match the existing project style.
-- When providing a multi-line commit description or detailed change list, use bullet points
   so each change is clearly itemized and easy to review.

## 6. Code Generation Standards
- **Language:** All generated code and internal comments MUST be in **English**.
- **Modern Practices:** Before suggesting any implementation, explicitly check for and prioritize modern .NET features (.NET 8/9/10) and C# language features (C# 12/13/14) to ensure cleaner, more performant, and idiomatic code (e.g., primary constructors, collection expressions, minimal APIs).
- **Tooling Limitations:** If a `dotnet build`, `dotnet test`, or any other terminal command fails because files are locked or the local debugger is running, do NOT silently ignore the failure. Attempt to stop the debugging session if possible, and if not, explicitly ask the user to stop the debugger and try again.
