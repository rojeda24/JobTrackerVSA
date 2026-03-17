using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using JobTrackerVSA.Web.Infrastructure.Auth;
using JobTrackerVSA.Web.Infrastructure.Storage;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public class AddJobApplicationHandler (AppDbContext context, ICurrentUserService currentUser, IResumeStorageService resumeStorageService, ILogger<AddJobApplicationHandler> logger) : IRequestHandler<AddJobApplicationCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddJobApplicationCommand command, CancellationToken cancellationToken)
        {
            var application = new JobApplication
            {
                UserId = currentUser.UserId ?? throw new UnauthorizedAccessException("User must be logged in"),
                CompanyName = command.CompanyName,
                Position = command.Position,
                JobDescriptionUrl = command.JobDescriptionUrl,
                Notes = command.Notes,
                CoverLetter = command.CoverLetter,
                Status = JobApplication.ApplicationStatus.Applied,
                AppliedAt = command.AppliedAt.HasValue 
                    ? DateTime.SpecifyKind(command.AppliedAt.Value, DateTimeKind.Utc) 
                    : DateTime.UtcNow
            };

            if (command.Resume is not null)
            {
                try
                {
                    var extension = Path.GetExtension(command.Resume.FileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    using var stream = command.Resume.OpenReadStream();
                    application.ResumeUrl = await resumeStorageService.UploadResumeAsync(stream, fileName, command.Resume.ContentType, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to upload resume for company {CompanyName}. File: {FileName}", command.CompanyName, command.Resume.FileName);
                    
                    // Catching generic Exception because Azure.RequestFailedException requires Azure.Core namespace
                    // and we might switch storage providers in the future.
                    return Result<Guid>.Failure("We couldn't upload your resume. Please try again later.");
                }
            }

            context.JobApplications.Add(application);
            await context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(application.Id);
        }
    }
}
