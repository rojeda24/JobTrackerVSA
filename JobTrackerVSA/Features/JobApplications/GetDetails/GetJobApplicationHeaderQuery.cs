using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.GetDetails
{
    public record GetJobApplicationHeaderQuery(Guid JobId) : IRequest<Result<JobApplicationHeaderViewModel>>;
}
