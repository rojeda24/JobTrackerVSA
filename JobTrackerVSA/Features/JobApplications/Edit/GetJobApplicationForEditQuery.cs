using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public record GetJobApplicationForEditQuery(Guid Id): IRequest<Result<EditModel.InputModel>>;
}