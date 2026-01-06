using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Register
{
    public record RegisterJobApplicationCommand : IRequest<Guid>
    {
        public required string CompanyName { get; init; }
        public required string Position { get; init; }
        public string? JobDescriptionUrl { get; init; }
        public DateTime? AppliedAt { get; init; }
        public string? Notes { get; init; }
    }
}
