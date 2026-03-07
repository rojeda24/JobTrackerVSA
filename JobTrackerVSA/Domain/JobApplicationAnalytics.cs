using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Domain
{
    /// <summary>
    /// Read-only projection used for analytics / reporting (Power BI).
    /// Backed by a SQL view.
    /// </summary>
    [Keyless]
    public sealed record JobApplicationAnalytics
    (
        Guid JobApplicationId,
        string UserId,
        string CompanyName,
        string Position,
        DateTime AppliedAt,
        DateTime AppliedDate,
        int AppliedDayOfWeek,
        string AppliedDayOfWeekName,
        int ApplicationStatus,
        string? ApplicationNotes,
        int InterviewCount,
        bool HasInterview,
        DateTime? FirstInterviewAt,
        DateTime? FirstInterviewDate,
        int? FirstInterviewDayOfWeek,
        string? FirstInterviewDayOfWeekName,
        DateTime? LastInterviewAt,
        DateTime? LastInterviewDate
    );
}
