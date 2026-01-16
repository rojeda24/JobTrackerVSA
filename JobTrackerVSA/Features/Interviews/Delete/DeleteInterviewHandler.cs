using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.Interviews.Delete
{
    public class DeleteInterviewHandler(AppDbContext context) : IRequestHandler<DeleteInterviewCommand, Result>
    {
        public async Task<Result> Handle(DeleteInterviewCommand request, CancellationToken cancellationToken)
        {
            var interview = await context.Interviews
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (interview == null)
            {
                return Result.Failure($"Interview with ID {request.Id} not found. No interview was removed.");
            }

            context.Interviews.Remove(interview);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception)
            {
                //TODO: LOG EXCEPTION
                return Result.Failure($"An unexpected error occurred when trying to delete Interview with ID {request.Id} in database.");
            }
        }
    }
}