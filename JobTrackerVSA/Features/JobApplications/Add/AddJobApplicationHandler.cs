using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public class AddJobApplicationHandler (AppDbContext context): IRequestHandler<AddJobApplicationCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddJobApplicationCommand command, CancellationToken cancellationToken)
        {
            var application = new JobApplication
            {
                CompanyName = command.CompanyName,
                Position = command.Position,
                JobDescriptionUrl = command.JobDescriptionUrl,
                Notes = command.Notes,
                Status = JobApplication.ApplicationStatus.Applied,
                AppliedAt = command.AppliedAt ?? DateTime.UtcNow
            };

            context.JobApplications.Add(application);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(application.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Unexpected error when trying to add new job application in database: {ex.ToString()}");
            }
        }
    }
}
