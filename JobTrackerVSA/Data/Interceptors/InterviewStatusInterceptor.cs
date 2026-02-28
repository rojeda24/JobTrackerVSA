using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using JobTrackerVSA.Web.Domain;

namespace JobTrackerVSA.Web.Data.Interceptors;

/// <summary>
/// EF Core interceptor that updates a JobApplication's Status when an Interview
/// is added or modified. Runs inside the SaveChanges pipeline so updates are
/// part of the same database transaction.
///
/// Mapping:
/// - InterviewType.Technical => ApplicationStatus.TechnicalTest
/// - InterviewType.Proposal  => ApplicationStatus.Offered
/// - Otherwise               => ApplicationStatus.Interviewing
/// </summary>
public sealed class InterviewStatusInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateJobApplicationStatus(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateJobApplicationStatus(DbContext? context)
    {
        if (context is not AppDbContext db)
            return;

        var changed = db.ChangeTracker
            .Entries<Interview>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToList();

        if (!changed.Any())
            return;

        // Collect distinct JobApplicationIds to avoid N+1 queries
        var appIds = changed.Select(i => i.JobApplicationId).Distinct().ToList();

        // Load tracked applications first
        var localApps = db.JobApplications.Local.Where(a => appIds.Contains(a.Id)).ToDictionary(a => a.Id);

        foreach (var interview in changed)
        {
            if (!localApps.TryGetValue(interview.JobApplicationId, out var app))
            {
                // Not tracked in memory -> try to query by primary key (no AsTracking needed)
                app = db.JobApplications.Find(interview.JobApplicationId);
                if (app != null)
                    localApps[app.Id] = app;
            }

            if (app is null)
                continue;

            app.Status = interview.Type switch
            {
                Interview.InterviewType.Technical => JobApplication.ApplicationStatus.TechnicalTest,
                Interview.InterviewType.Proposal => JobApplication.ApplicationStatus.Offered,
                _ => JobApplication.ApplicationStatus.Interviewing
            };
        }
    }
}
