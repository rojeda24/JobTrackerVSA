# JobTrackerVSA

A modern Vertical Slice Architecture Web App for tracking job applications and interviews, built with ASP.NET Core 10 and SQL Server.

## 🟢 Live Demo
**Try the app here:** [https://jobtracker-raul.azurewebsites.net/](https://jobtracker-raul.azurewebsites.net/)

![Demo](https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExMXhnZDh0NXFuanNtZWlkb20ycnBkdG9idzd5MzkyNHFqYmNsZmY0OSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/disN31EHucKJdGQalA/giphy.gif)

## Architecture
- **Vertical Slice Architecture:** Logic organized by Features (`Features/JobApplications/Add`, etc.).
- **CQRS:** Using MediatR for clean separation of commands and queries.
- **Unit Testing:** Comprehensive test suite for all features using xUnit and EF Core In-Memory(DB).
- **Observability:** Centralized exception logging via MediatR Pipeline Behavior.
- **Security:** "Secure by Default" policy. All endpoints require authentication unless marked `[AllowAnonymous]`.
- **Cloud Storage:** Secure document storage (Resumes/CVs) integration using Azure Blob Storage with strict server-side validation (Magic Numbers checking) and private-by-default access policies.

### Architecture Diagram

```mermaid
graph TD
    Client([Client Browser])
    Auth0([Auth0 Identity Provider])
    WebTest([App Insights Web Test<br/>Ping /health])

    subgraph "Azure Environment (infrastructure/main.bicep)"
        subgraph "App Service Plan (Linux F1)"
            subgraph "Azure Web App (JobTrackerVSA .NET 10)"
                Auth[Auth & Exception Middleware]
                MediatR{MediatR Pipeline}
                
                subgraph "Features (Vertical Slices)"
                    direction TB
                    Slice1["📁 Job Apps"]
                    Slice2["📁 Interviews"]
                    Slice3["📁 Dashboard"]
                end
                
                EFCore[EF Core DbContext]
                StorageSvc[Storage Service]
            end
        end

        subgraph "Data & Storage"
            SQLDB[(Azure SQL DB<br/>Standard S0)]
            Blob[(Azure Blob Storage<br/>Container: 'resumes')]
        end

        subgraph "Observability"
            AppInsights[Application Insights]
            LogAnalytics[Log Analytics Workspace]
        end
    end

    Client -->|HTTPS Request| Auth
    Auth <-->|OIDC Auth| Auth0
    WebTest -->|Ping every 15m| Auth
    
    Auth -->|Route| Slice1
    Auth -->|Route| Slice2
    Auth -->|Route| Slice3
    
    Slice1 -.->|Send| MediatR
    Slice2 -.->|Send| MediatR
    Slice3 -.->|Send| MediatR
    
    MediatR -.->|Dispatch| Slice1
    MediatR -.->|Dispatch| Slice2
    MediatR -.->|Dispatch| Slice3
    
    Slice1 -->|Read/Write| EFCore
    Slice2 -->|Read/Write| EFCore
    Slice3 -->|Read| EFCore
    
    Slice1 -->|Upload/Read| StorageSvc
    
    EFCore -->|SQL Connection| SQLDB
    StorageSvc -->|Azure SDK| Blob
    
    Auth -.->|Telemetry/Logs| AppInsights
    AppInsights -->|Store| LogAnalytics
```

## API Documentation
For detailed API information, interactive testing, and schema definitions, visit our **OpenAPI Documentation** powered by [Scalar](https://github.com/scalar/scalar) (a modern alternative to Swagger UI).

👉 **[View API Docs](https://jobtracker-raul.azurewebsites.net/scalar)**

## 🚀 Deployment & DevOps
This project implements a professional, fully automated **CI/CD pipeline** targeting **Microsoft Azure**.

### Infrastructure as Code (IaC)
- **Azure Bicep:** The entire infrastructure (Linux App Service, SQL Server/Database, Firewall Rules) is defined in code (`infrastructure/main.bicep`).
- **Reproducible:** The environment can be recreated from scratch automatically.

### CI/CD Pipeline (GitHub Actions)
- **Secure Authentication:** Uses **OpenID Connect (OIDC)** for "passwordless" security between GitHub and Azure (No Client Secrets stored).
- **Automated Workflow:**
  1.  **Infrastructure Job:** Validates and applies Bicep templates.
  2.  **Build Job:** Compiles .NET 10.
  3.  **Test Job:** Runs unit tests automatically to ensure quality before deployment.
  4.  **Deploy Job:** Deploys to Azure Web App.
  5.  **Auto-Migration:** Database schemas are applied automatically on application startup.

```mermaid
graph TD
    Dev([Developer]) -->|Push to main| GitHub[GitHub Repository]
    
    subgraph "GitHub Actions Workflow (deploy-azure.yml)"
        direction TB
        
        subgraph Job1["Job 1: Infrastructure"]
            Login1["🔐 Azure Login (OIDC)"] --> Bicep["⚙️ Deploy Infrastructure (Bicep Template)"]
        end
        
        subgraph Job2["Job 2: Build and Deploy"]
            Login2["🔐 Azure Login (OIDC)"] --> Build["🛠️ Build .NET 10"]
            Build --> Test["🧪 Run Unit Tests"]
            Test --> Migrations["🗄️ Run EF Core Migrations<br/>"]
            Migrations --> Libman["📦 Restore Client Libs (LibMan)"]
            Libman --> Publish["🚀 Publish & Deploy"]
        end
    end
    
    GitHub -->|Trigger| Job1
    Job1 --> Job2
    Publish -->|Release| Azure([Azure App Service])
    Migrations -->|Update DB| DB[(Azure SQL Database)]
    
    classDef pass stroke:#2ea043,stroke-width:2px;
    class Test pass;
```

### Automated Demo Environment Maintenance
To ensure the public live demo always provides a clean experience for new visitors, a **GitHub Actions Cron Job** (`cleanup-demo-user.yml`) runs daily at 10:00 UTC.
- It securely calls the `/api/maintenance/reset-demo` endpoint using an API Key.
- The endpoint automatically wipes all data associated with the demo user.
- It re-seeds the database with fresh, realistic sample data (job applications and interviews) ensuring a consistent demo state every day.
- 
# 💻 Local Environment Installation

## Prerequisites

- **.NET 10 SDK**
- **Visual Studio Code** (with C# Dev Kit extension)
- **SQL Server LocalDB** (or a Docker SQL Server container)
- **Auth0 Account** (free tier is fine)
- **Node.js & npm** (Required to run the Azurite Storage Emulator)

### Recommended VS Code Extensions
For the best development experience in this project, install the following extensions:
- **C# Dev Kit** (`ms-dotnettools.csdevkit`): Official C# support, Solution Explorer, and Test Explorer.
- **C#** (`ms-dotnettools.csharp`): Base language support (installed automatically with Dev Kit).
- **Azurite** (`Azurite.azurite`): Essential for local Azure Blob Storage emulation.
- **Bicep** (`ms-azuretools.vscode-bicep`): Required for editing Infrastructure as Code files.
- **GitHub Actions** (`github.vscode-github-actions`): Helpful for managing CI/CD pipelines.
- **Mermaid Chart** (`mermaidchart.vscode-mermaid-chart`): Official Mermaid extension to seamlessly create, edit, and preview Mermaid diagrams directly in VS Code.

## Quick Start

### 1. Database Setup
The project is configured to use **SQL Server LocalDB** (`(localdb)\mssqllocaldb`) by default. If you don't have Visual Studio installed, you can download Microsoft SQL Server Express LocalDB separately, or update the connection string in `appsettings.Local.json` to point to a Docker container.

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
4.  Ensure **Azurite** is installed globally via npm (`npm install -g azurite`). The VS Code launch configuration will automatically start it in the background when you run the project.

5. Restore client-side libraries, apply database migrations, and start the server:
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
