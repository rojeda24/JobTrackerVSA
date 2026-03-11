using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace JobTrackerVSA.Web.Features.JobApplications.Add
{
    public record AddJobApplicationCommand : IRequest<Result<Guid>>
    {
        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(150, ErrorMessage = "Company Name cannot exceed 150 characters.")]
        public required string CompanyName { get; init; }

        [Required(ErrorMessage = "Position is required.")]
        [StringLength(150, ErrorMessage = "Position cannot exceed 150 characters.")]
        public required string Position { get; init; }

        public string? JobDescriptionUrl { get; init; }
        public DateTime? AppliedAt { get; init; }
        public string? Notes { get; init; }
    }
}
