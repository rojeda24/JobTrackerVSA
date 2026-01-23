# JobTrackerVSA

A modern Vertical Slice Architecture application for tracking job applications and interviews, built with ASP.NET Core 10 and SQL Server LocalDB.

## Prerequisites

- **.NET 10 SDK**
- **Visual Studio 2022** (or SQL Server LocalDB installed separately)
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

### 4. Run the App
Restore client-side libraries, apply database migrations, and start the server:
```bash
# Restore CSS/JS libraries (Bootstrap, etc.)
libman restore

# Update DB and Run
dotnet ef database update --project JobTrackerVSA.Web.csproj
dotnet run --project JobTrackerVSA.Web.csproj
```

Visit `https://localhost:7206`. You will be redirected to Auth0 to log in.

## Architecture
- **Vertical Slice Architecture:** Logic organized by Features (`Features/JobApplications/Add`, etc.).
- **CQRS:** Using MediatR for clean separation of commands and queries.
- **Security:** "Secure by Default" policy. All endpoints require authentication unless marked `[AllowAnonymous]`.
