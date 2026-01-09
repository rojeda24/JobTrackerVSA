using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using static JobTrackerVSA.Web.Domain.JobApplication;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
    public record GetJobApplicationsQuery : IRequest<Result<List<JobApplicationSummaryViewModel>>>;
}
