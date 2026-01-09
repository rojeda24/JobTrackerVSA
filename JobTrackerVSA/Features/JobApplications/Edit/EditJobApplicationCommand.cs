using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using static JobTrackerVSA.Web.Domain.JobApplication;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public record EditJobApplicationCommand
    (
        Guid Id,
        string CompanyName,
        string Position,
        string? JobDescriptionUrl,
        DateTime AppliedAt,
        ApplicationStatus Status,
        string? Notes
    ) : IRequest<Result>;
}