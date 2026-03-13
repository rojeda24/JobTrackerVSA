using JobTrackerVSA.Web.Features.Interviews.Shared;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;
using static JobTrackerVSA.Web.Domain.Interview;

namespace JobTrackerVSA.Web.Features.Interviews.Edit;

public record EditInterviewCommand
(
    Guid Id,
    Guid JobApplicationId,
    DateTime ScheduledAt,
    InterviewType Type,
    [StringLength(4000, ErrorMessage = "Notes cannot exceed 4000 characters.")]
    string? Notes
) : IRequest<Result>, IInterviewForm;
