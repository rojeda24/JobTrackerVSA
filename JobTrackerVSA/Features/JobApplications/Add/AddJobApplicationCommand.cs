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

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(2048, ErrorMessage = "Job Description URL cannot exceed 2048 characters.")]
        public string? JobDescriptionUrl { get; init; }
        
        public DateTime? AppliedAt { get; init; }

        [StringLength(4000, ErrorMessage = "Notes cannot exceed 4000 characters.")]
        public string? Notes { get; init; }

        [StringLength(4000, ErrorMessage = "Cover letter cannot exceed 4000 characters.")]
        public string? CoverLetter { get; init; }
    }
}
