using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Register
{
    public class RegisterJobApplicationHandler (AppDbContext context): IRequestHandler<RegisterJobApplicationCommand, Guid>
    {
        public async Task<Guid> Handle(RegisterJobApplicationCommand command, CancellationToken cancellationToken)
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
