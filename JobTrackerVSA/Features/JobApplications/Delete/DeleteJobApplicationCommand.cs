using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Delete
{
    public record DeleteJobApplicationCommand(Guid Id) : IRequest<Result>;
}
