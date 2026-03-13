using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Interviews.Shared;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace JobTrackerVSA.Web.Features.Interviews.Add;

public record AddInterviewCommand(
    Guid JobApplicationId,
    DateTime ScheduledAt,
    Interview.InterviewType Type,
    [StringLength(4000, ErrorMessage = "Notes cannot exceed 4000 characters.")]
    string? Notes
):IRequest<Result<Guid>>, IInterviewForm;
