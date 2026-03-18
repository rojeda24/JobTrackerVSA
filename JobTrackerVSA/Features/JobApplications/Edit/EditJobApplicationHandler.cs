using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using JobTrackerVSA.Web.Infrastructure.Storage;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit;

public class EditJobApplicationHandler (AppDbContext context, IResumeStorageService resumeStorageService, ILogger<EditJobApplicationHandler> logger)
: IRequestHandler<EditJobApplicationCommand, Result>
{
    public async Task<Result> Handle(EditJobApplicationCommand command, CancellationToken cancellationToken) 
    {
        var app = await context.JobApplications
            .FindAsync([command.Id], cancellationToken);

        if (app == null) 
            return Result.Failure($"No Job Application found with ID {command.Id}");

        if (command.Resume is not null)
        {
            try
            {
                var extension = Path.GetExtension(command.Resume.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                using var stream = command.Resume.OpenReadStream();
                
                // 1. Upload the new file FIRST
                var newResumeUrl = await resumeStorageService.UploadResumeAsync(stream, fileName, command.Resume.ContentType, cancellationToken);
                
                // 2. If upload was successful, delete the old file
                if (!string.IsNullOrEmpty(app.ResumeUrl))
                {
                    await resumeStorageService.DeleteResumeAsync(app.ResumeUrl, cancellationToken);
                }

                // 3. Update the entity with the new URL
                app.ResumeUrl = newResumeUrl;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload new resume for JobApplication ID {Id}. File: {FileName}", command.Id, command.Resume.FileName);
                return Result.Failure("We couldn't upload your new resume. Please try again later.");
            }
        }

        app.CompanyName = command.CompanyName;
        app.Position = command.Position;
        app.JobDescriptionUrl = command.JobDescriptionUrl;
        app.AppliedAt = DateTime.SpecifyKind(command.AppliedAt, DateTimeKind.Utc);
        app.Status = command.Status;
        app.Notes = command.Notes;
        app.CoverLetter = command.CoverLetter;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
