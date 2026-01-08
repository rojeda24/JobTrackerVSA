using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public record GetJobApplicationForEditQuery(Guid Id): IRequest<EditModel.InputModel?>;
}
