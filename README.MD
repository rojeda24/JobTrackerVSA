# JobTrackerVSA

A modern Vertical Slice Architecture Web App for tracking job applications and interviews, built with ASP.NET Core 10 and SQL Server.

## 🟢 Live Demo
**Try the app here:** [https://jobtracker-raul.azurewebsites.net/](https://jobtracker-raul.azurewebsites.net/)

![Demo](https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExMXhnZDh0NXFuanNtZWlkb20ycnBkdG9idzd5MzkyNHFqYmNsZmY0OSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/disN31EHucKJdGQalA/giphy.gif)

## Architecture
- **Vertical Slice Architecture:** Logic organized by Features (`Features/JobApplications/Add`, etc.).
- **CQRS:** Using MediatR for clean separation of commands and queries.
- **Security:** "Secure by Default" policy. All endpoints require authentication unless marked `[AllowAnonymous]`.

## 🚀 Deployment & DevOps
This project implements a professional, fully automated **CI/CD pipeline** targeting **Microsoft Azure**.

### Infrastructure as Code (IaC)
- **Azure Bicep:** The entire infrastructure (Linux App Service, SQL Server/Database, Firewall Rules) is defined in code (`infrastructure/main.bicep`).
- **Reproducible:** The environment can be recreated from scratch automatically.

### CI/CD Pipeline (GitHub Actions)
- **Secure Authentication:** Uses **OpenID Connect (OIDC)** for "passwordless" security between GitHub and Azure (No Client Secrets stored).
- **Automated Workflow:**
  1.  **Infrastructure Job:** Validates and applies Bicep templates.
  2.  **Build & Deploy Job:** Compiles .NET 10, and deploys to Azure Web App.
  3.  **Auto-Migration:** Database schemas are applied automatically on application startup.

# 💻 Local Environment Installation

## Prerequisites

- **.NET 10 SDK**
- **Visual Studio 2026** (or SQL Server LocalDB installed separately)
- **Auth0 Account** (free tier is fine)

## Quick Start

### 1. Database Setup
The project is configured to use **SQL Server LocalDB** (`(localdb)\mssqllocaldb`) by default, which usually comes installed with Visual Studio's ASP.NET and web development workload. No manual database server setup is required.

### 2. Authentication Setup (Auth0)
1.  Go to [Auth0 Dashboard](https://manage.auth0.com/).
2.  Create a **Regular Web Application**.
3.  Set Allowed Callback/Logout URLs to: `https://localhost:7206/callback` and `https://localhost:7206/`.
4.  Copy your **Domain** and **Client ID**.

### 3. Local Configuration
1.  Go to the project folder: `JobTrackerVSA/`.
2.  Copy the example configuration file:
    ```bash
    cp appsettings.Local.example.json appsettings.Local.json
    ```
    *(Or just copy/paste and rename manually)*
3.  Open `appsettings.Local.json` and paste your Auth0 credentials:
    ```json
    {
      "Auth0": {
        "Domain": "your-domain.us.auth0.com",
        "ClientId": "your-client-id"
      }
    }
    ```

4. Restore client-side libraries, apply database migrations, and start the server:
```bash
# Install Entity Framework Core tools and libman if not already installed
dotnet tool install --global dotnet-ef
dotnet tool install --global Microsoft.Web.LibraryManager.Cli

# Restore CSS/JS libraries (Bootstrap, etc.)
libman restore

# Update DB and Run
dotnet ef database update --project JobTrackerVSA.Web.csproj
dotnet run --project JobTrackerVSA.Web.csproj
```

### 4. Run the App

Run the app (F5) and visit `https://localhost:7206`. You will be redirected to Auth0 to log in.
