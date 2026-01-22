namespace JobTrackerVSA.Web.Domain
{
    public class JobApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserId { get; set; }
        public required string CompanyName { get; set; }
        public required string Position { get; set; }
        public string? JobDescriptionUrl { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public ApplicationStatus Status { get; set; }
        public string? Notes { get; set; }

        public List<Interview> Interviews { get; set; } = [];

        public enum ApplicationStatus
        {
            Applied,
            Interviewing,
            TechnicalTest,
            Offered,
            Rejected,
            Accepted
        }
    }
}
