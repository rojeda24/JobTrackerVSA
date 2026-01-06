using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public class AddJobApplicationHandler (AppDbContext context): IRequestHandler<AddJobApplicationCommand, Guid>
    {
        public async Task<Guid> Handle(AddJobApplicationCommand command, CancellationToken cancellationToken)
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
            await context.SaveChangesAsync(cancellationToken);

            return application.Id;
        }
    }
}
