using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.Interviews.Add
{
    public class AddInterviewHandler(AppDbContext context) : IRequestHandler<AddInterviewCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddInterviewCommand command, CancellationToken cancellationToken)
        {
            var applicationExists = await context.JobApplications
                .AnyAsync(a => a.Id == command.JobApplicationId, cancellationToken);

            if (!applicationExists)
                return Result<Guid>.Failure($"Job application not found with id {command.JobApplicationId}");

            var interview = new Interview
            {
                JobApplicationId = command.JobApplicationId,
                ScheduledAt = command.ScheduledAt,
                Type = command.Type,
                Notes = command.Notes
            };

            try
            {
                context.Interviews.Add(interview);
                await context.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(interview.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure($"An error occurred while scheduling the interview for JobApplication with ID={command.JobApplicationId}");
            }
        }
    }
}
