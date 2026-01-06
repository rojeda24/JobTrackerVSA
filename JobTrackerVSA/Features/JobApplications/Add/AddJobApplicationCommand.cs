using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public record AddJobApplicationCommand : IRequest<Guid>
    {
        public required string CompanyName { get; init; }
        public required string Position { get; init; }
        public string? JobDescriptionUrl { get; init; }
        public DateTime? AppliedAt { get; init; }
        public string? Notes { get; init; }
    }
}
