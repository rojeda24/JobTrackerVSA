using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.Interviews.Edit
{
    public class EditInterviewHandler(AppDbContext context)
        : IRequestHandler<EditInterviewCommand, Result>
    {
        public async Task<Result> Handle(EditInterviewCommand command, CancellationToken cancellationToken)
        {
            var interview = await context.Interviews.FindAsync([command.Id], cancellationToken);
            if (interview == null) return Result<Unit>.Failure($"Interview not found with ID {command.Id}");

            interview.ScheduledAt = command.ScheduledAt;
            interview.Type = command.Type;
            interview.Notes = command.Notes;

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Unexpected error when trying to save Interview with ID {command.Id} in database: {ex.ToString()}");
            }
        }
    }
}
