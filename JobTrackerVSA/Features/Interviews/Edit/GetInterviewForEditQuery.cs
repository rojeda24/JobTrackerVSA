using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.Interviews.Edit
{
    public record GetInterviewForEditQuery(Guid Id) : IRequest<Result<EditInterviewCommand>>; //Change EditInterviewCommand to a new ViewModel/DTO if different data is needed
}
