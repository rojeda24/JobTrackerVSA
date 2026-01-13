namespace JobTrackerVSA.Web.Domain
{
    public class Interview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid JobApplicationId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public InterviewType Type { get; set; }
        public string? Notes { get; set; }

        public JobApplication JobApplication { get; set; } = null!;

        public enum InterviewType
        {
            General,
            HR,
            Technical,
            Proposal
        }
    }
}
