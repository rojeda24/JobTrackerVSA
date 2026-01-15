using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using static JobTrackerVSA.Web.Domain.Interview;

namespace JobTrackerVSA.Web.Features.Interviews.Edit
{
    public record EditInterviewCommand
    (
        Guid Id,
        Guid JobApplicationId,
        DateTime ScheduledAt,
        InterviewType Type,
        string? Notes
    ) : IRequest<Result>;
}
