using static JobTrackerVSA.Web.Domain.Interview;

namespace JobTrackerVSA.Web.Features.Interviews.Shared
{
    public interface IInterviewForm
    {
        DateTime ScheduledAt { get; }
        InterviewType Type { get; }
        string? Notes { get; }
    }
}
