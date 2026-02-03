using JobTrackerVSA.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.Maintenance;

public static class MaintenanceEndpoints
{
    public static void MapMaintenanceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/maintenance/reset-demo", HandleResetDemo)
           .WithTags("Maintenance")
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

        // Use IgnoreQueryFilters to bypass the global filter (UserId == currentUserService.UserId)
        // because in this context (external API call), there is no authenticated user.
        var deletedCount = await db.JobApplications
            .IgnoreQueryFilters()
            .Where(j => j.UserId == demoUserId)
            .ExecuteDeleteAsync();

        return Results.Ok(new { message = $"Cleanup completed. {deletedCount} applications deleted." });
    }
}
