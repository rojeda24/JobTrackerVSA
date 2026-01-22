using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using JobTrackerVSA.Web.Infrastructure.Auth;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public class AddJobApplicationHandler (AppDbContext context, ICurrentUserService currentUser) : IRequestHandler<AddJobApplicationCommand, Result<Guid>>
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
                Status = JobApplication.ApplicationStatus.Applied,
                AppliedAt = command.AppliedAt.HasValue 
                    ? DateTime.SpecifyKind(command.AppliedAt.Value, DateTimeKind.Utc) 
                    : DateTime.UtcNow
            };

            context.JobApplications.Add(application);

            //try
            //{
                await context.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(application.Id);
            //}
            //catch (DbUpdateException)
            //{
            //    //TODO: LOG EXCEPTION
            //    return Result<Guid>.Failure($"Unexpected error when trying to add new job application in database");
            //}
        }
    }
}
