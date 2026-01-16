using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Interviews.Shared;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.Interviews.Add
{
    public record AddInterviewCommand(
        Guid JobApplicationId,
        DateTime ScheduledAt,
        Interview.InterviewType Type,
        string? Notes
    ):IRequest<Result<Guid>>, IInterviewForm;
}
