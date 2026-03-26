using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using Microsoft.EntityFrameworkCore;
using static JobTrackerVSA.Web.Domain.JobApplication;
using static JobTrackerVSA.Web.Domain.Interview;

namespace JobTrackerVSA.Web.Features.Maintenance;

public static class MaintenanceEndpoints
{
    public static void MapMaintenanceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/maintenance/reset-demo", HandleResetDemo)
           .WithTags("Maintenance")
           .WithSummary("Resets the demo environment data")
           .WithDescription("""
                **Automated Demo Reset**
                
                To ensure the public demo always has clean data for new visitors, this endpoint is designed to be called by a Cron Job (e.g., GitHub Actions).
                
                **Function:**
                1.  **Deletes** all data associated with the public 'Demo User'.
                2.  **Seeds** fresh, realistic sample data (including English/Spanish examples).
                
                **Security:**
                This endpoint is guarded by a secret API Key (`X-Maintenance-Key` header).
                """)
           .Produces(200)
           .Produces(401)
           .AllowAnonymous();

    }

    internal static async Task<IResult> HandleResetDemo(
        AppDbContext db,
        IConfiguration config,
        HttpContext context)
    {
        var requestKey = context.Request.Headers["X-Maintenance-Key"].FirstOrDefault();
        var configKey = config["Maintenance:ApiKey"];

        if (string.IsNullOrEmpty(configKey) || requestKey != configKey)
        {
            return Results.Unauthorized();
        }

        var demoUserId = config["Maintenance:DemoUserId"];
        if (string.IsNullOrEmpty(demoUserId))
        {
            return Results.Problem("Demo User ID not configured.");
        }

        // 1. Cleanup: Delete existing data
        var deletedCount = await db.JobApplications
            .IgnoreQueryFilters() // To bypass the global filter (UserId == currentUserService.UserId)
            .Where(j => j.UserId == demoUserId)
            .ExecuteDeleteAsync();

        // 2. Seed: Repopulate with fresh data
        var seedData = GetSeedData(demoUserId);
        db.JobApplications.AddRange(seedData);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = $"Reset completed. {deletedCount} items deleted, {seedData.Count} items seeded." });
    }

    internal static List<JobApplication> GetSeedData(string userId)
    {
        var today = DateTime.UtcNow;

        return
        [
            new()
            {
                UserId = userId,
                CompanyName = "TechGiant Corp",
                Position = "Senior .NET Developer",
                Status = ApplicationStatus.Interviewing,
                AppliedAt = today.AddDays(-14),
                JobDescriptionUrl = "https://careers.techgiant.example/jobs/123",
                Notes = "Referral from Sarah. Great benefits package.",
                CoverLetter = "I am excited to apply for this Senior .NET Developer role.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-5), Type = InterviewType.HR, Notes = "Cultural fit interview" },
                    new() { ScheduledAt = today.AddDays(2), Type = InterviewType.Technical, Notes = "System Design round" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "StartupX",
                Position = "Full Stack Engineer",
                Status = ApplicationStatus.TechnicalTest,
                AppliedAt = today.AddDays(-7),
                Notes = "Remote first culture. Using React and Node.js.",
                CoverLetter = "With my background in React and Node.js, I'd love to join your team.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-2), Type = InterviewType.General, Notes = "Intro with CTO" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "Legacy Bank",
                Position = "Backend Architect",
                Status = ApplicationStatus.Rejected,
                AppliedAt = today.AddDays(-30),
                Notes = "They required 5 days in office.",
                CoverLetter = "I have 10 years of experience building secure backend architectures.",
                Interviews = []
            },
            new()
            {
                UserId = userId,
                CompanyName = "CloudSystems Inc",
                Position = "DevOps Engineer",
                Status = ApplicationStatus.Applied,
                AppliedAt = today.AddDays(-2),
                Notes = "Applied via LinkedIn Easy Apply.",
                CoverLetter = "My passion for automation makes me a great fit for DevOps.",
                Interviews = []
            },
            new()
            {
                UserId = userId,
                CompanyName = "AutoParts Mexico",
                Position = "Ingeniero de Calidad Jr",
                Status = ApplicationStatus.Applied,
                AppliedAt = today.AddDays(-5),
                Notes = "Ubicación: Planta Saltillo. Requieren inglés conversacional.",
                CoverLetter = "Me interesa mucho la vacante de Ingeniero de Calidad.",
                Interviews = []
            },
            new()
            {
                UserId = userId,
                CompanyName = "FutureMotors EV",
                Position = "Desarrollador de Software Embebido",
                Status = ApplicationStatus.Interviewing,
                AppliedAt = today.AddDays(-10),
                JobDescriptionUrl = "https://futuremotors.mx/carreras/embedded-dev",
                Notes = "Proyecto de vehículos autónomos.",
                CoverLetter = "Tengo gran interés en el desarrollo de software para vehículos autónomos.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-2), Type = InterviewType.HR, Notes = "Entrevista inicial con RH" },
                    new() { ScheduledAt = today.AddDays(3), Type = InterviewType.Technical, Notes = "Prueba técnica de C++ y RTOS" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "Global Solutions LLC",
                Position = "Project Manager",
                Status = ApplicationStatus.Offered,
                AppliedAt = today.AddDays(-20),
                Notes = "Great benefits package and fully remote.",
                CoverLetter = "I am an experienced PM looking to drive global solutions.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-15), Type = InterviewType.HR, Notes = "Screening" },
                    new() { ScheduledAt = today.AddDays(-5), Type = InterviewType.General, Notes = "Final interview with VP" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "Fintech LATAM",
                Position = "Data Analyst",
                Status = ApplicationStatus.Rejected,
                AppliedAt = today.AddDays(-40),
                Notes = "Decided to go with an internal candidate.",
                CoverLetter = "Tengo experiencia analizando datos financieros.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-35), Type = InterviewType.HR, Notes = "Screening inicial" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "EduTech Platform",
                Position = "Frontend Developer",
                Status = ApplicationStatus.Interviewing,
                AppliedAt = today.AddDays(-8),
                Notes = "Vue.js stack.",
                CoverLetter = "I have been building educational platforms for 3 years.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-1), Type = InterviewType.Technical, Notes = "Live coding session" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "HealthCore Systems",
                Position = "QA Automation Engineer",
                Status = ApplicationStatus.Accepted,
                AppliedAt = today.AddDays(-45),
                Notes = "Found on Indeed.",
                CoverLetter = "I specialize in Cypress and Selenium automation.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-40), Type = InterviewType.General, Notes = "Team fit" },
                    new() { ScheduledAt = today.AddDays(-35), Type = InterviewType.Technical, Notes = "Test framework implementation" }
                ]
            },
            new()
            {
                UserId = userId,
                CompanyName = "CyberShield Security",
                Position = "Security Analyst",
                Status = ApplicationStatus.TechnicalTest,
                AppliedAt = today.AddDays(-15),
                Notes = "Take-home CTF challenge.",
                CoverLetter = "Passionate about network security and ethical hacking.",
                Interviews =
                [
                    new() { ScheduledAt = today.AddDays(-10), Type = InterviewType.HR, Notes = "Introduction" }
                ]
            }
        ];
    }
}
