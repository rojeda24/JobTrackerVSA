using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.Interviews.Delete
{
    public record DeleteInterviewCommand(Guid Id) : IRequest<Result>;
}