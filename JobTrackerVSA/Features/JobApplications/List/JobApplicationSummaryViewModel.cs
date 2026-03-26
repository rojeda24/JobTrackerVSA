using static JobTrackerVSA.Web.Domain.JobApplication;

namespace JobTrackerVSA.Web.Features.JobApplications.List;

public record JobApplicationSummaryViewModel
{
    public Guid Id { get; set; }
    public string CompanyName { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string? JobDescriptionUrl { get; init; }
    public string FinalJobUrl => string.IsNullOrWhiteSpace(JobDescriptionUrl)
        ? "#"
        : JobDescriptionUrl.StartsWith("http")
            ? JobDescriptionUrl
            : $"https://{JobDescriptionUrl}";
    public DateTime? NextInterviewAt { get; init; }
    public ApplicationStatus Status { get; init; }
    public string? ResumeUrl { get; init; }

    public string StatusDisplay => Status switch
    {
        ApplicationStatus.Interviewing => "In interview",
        ApplicationStatus.TechnicalTest => "In technical test",
        ApplicationStatus.Offered => "Offer received",
        _ => Status.ToString()
    };

    private readonly string? _notes;
    public string? Notes 
    { 
        get => string.IsNullOrWhiteSpace(_notes) ? "---" : _notes;
        init => _notes = value;
    }
}
