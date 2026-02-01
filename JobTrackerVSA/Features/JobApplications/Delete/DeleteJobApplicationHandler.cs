using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.Delete
{
    public class DeleteJobApplicationHandler(AppDbContext context) : IRequestHandler<DeleteJobApplicationCommand, Result>
    {
        public async Task<Result> Handle(DeleteJobApplicationCommand command, CancellationToken cancellationToken)
        {
            var application = await context.JobApplications
                .FirstOrDefaultAsync(j => j.Id == command.Id, cancellationToken);

            if (application == null)
                return Result.Failure($"Job application with ID {command.Id} was not found. No job application was removed.");

            context.JobApplications.Remove(application);

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
