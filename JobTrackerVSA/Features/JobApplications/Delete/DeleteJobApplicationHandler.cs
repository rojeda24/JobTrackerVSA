using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using JobTrackerVSA.Web.Infrastructure.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.Delete;

public class DeleteJobApplicationHandler(AppDbContext context, IResumeStorageService resumeStorageService, ILogger<DeleteJobApplicationHandler> logger) : IRequestHandler<DeleteJobApplicationCommand, Result>
{
    public async Task<Result> Handle(DeleteJobApplicationCommand command, CancellationToken cancellationToken)
    {
        var application = await context.JobApplications
            .FirstOrDefaultAsync(j => j.Id == command.Id, cancellationToken);

        if (application == null)
            return Result.Failure($"Job application with ID {command.Id} was not found. No job application was removed.");

        if (!string.IsNullOrEmpty(application.ResumeUrl))
        {
            try
            {
                await resumeStorageService.DeleteResumeAsync(application.ResumeUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete resume blob for JobApplication ID {Id}. URL: {ResumeUrl}. The application record will still be deleted.", command.Id, application.ResumeUrl);
                // We do not return a failure here because we still want the DB record to be deleted
                // even if the blob storage cleanup fails.
            }
        }

        context.JobApplications.Remove(application);

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
