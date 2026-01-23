# AI Context - JobTrackerVSA

This file documents the architectural decisions, technical stack, and current state of the project to facilitate context loading for future AI sessions.

## 1. Project Overview
**JobTrackerVSA** is a vertical slice architecture application for tracking job applications and interviews.

### Tech Stack
- **Framework:** .NET 10 (ASP.NET Core Razor Pages)
- **Database:** MS SQL Server LocalDB (`(localdb)\mssqllocaldb`)
- **ORM:** Entity Framework Core (SQL Server)
- **Authentication:** Auth0 (OpenID Connect)
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

## 3. Key File Locations
- **DbContext:** `Data/AppDbContext.cs`
- **Auth Service:** `Infrastructure/Auth/CurrentUserService.cs`
- **Login/Logout:** `Features/Account/`
- **JS Utilities:** `wwwroot/js/site.js`

## 4. Workflow Guidelines for AI
- **Git Workflow:** 
  - **DO NOT** stage files (`git add`).
  - **DO NOT** commit changes (`git commit`).
  - The user reviews all changes in the "Unstaged" state and manually stages/commits them.
  - Role ends after `write`/`edit` operations are confirmed successful via compilation/verification.
