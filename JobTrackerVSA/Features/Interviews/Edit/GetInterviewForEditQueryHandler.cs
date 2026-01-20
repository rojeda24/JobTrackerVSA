using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.Interviews.Edit
{
    public class GetInterviewForEditQueryHandler(AppDbContext context)
        : IRequestHandler<GetInterviewForEditQuery, Result<EditInterviewCommand>>
    {
        public async Task<Result<EditInterviewCommand>> Handle(GetInterviewForEditQuery request, CancellationToken cancellationToken) //Change EditInterviewCommand to a new ViewModel/DTO if different data is needed
        {
            var interview = await context.Interviews
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (interview == null) return Result<EditInterviewCommand>.Failure($"Interview with ID {request.Id} not found.");

            var command = new EditInterviewCommand(
                interview.Id,
                interview.JobApplicationId,
                DateTime.SpecifyKind(interview.ScheduledAt, DateTimeKind.Utc),
                interview.Type,
                interview.Notes
            );

            return Result<EditInterviewCommand>.Success(command);
        }
    }
}
