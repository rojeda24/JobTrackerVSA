using MediatR;
using static JobTrackerVSA.Web.Domain.JobApplication;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
    public record GetJobApplicationsQuery : IRequest<List<JobApplicationSummaryViewModel>>;
}
