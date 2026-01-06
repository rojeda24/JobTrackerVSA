using static JobTrackerVSA.Web.Domain.JobApplication;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
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
        public DateTime AppliedAt { get; init; }
        public ApplicationStatus Status { get; init; }

        public string StatusDisplay => Status switch
        {
            ApplicationStatus.Interviewing => "In interview",
            ApplicationStatus.TechnicalTest => "In technical test",
            ApplicationStatus.Offered => "Offer received",
            _ => Status.ToString()
        };

        public string? Notes { get; init; }
        public string NotesPreview => string.IsNullOrWhiteSpace(Notes)
            ? "---"
            : Notes.Length <= 20
                ? Notes
                : $"{Notes.Substring(0, 20)}...";
    }
}
