using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.GetDetails
{
    public class GetJobApplicationHeaderHandler(AppDbContext context) : IRequestHandler<GetJobApplicationHeaderQuery, Result<JobApplicationHeaderViewModel>>
    {
        public async Task<Result<JobApplicationHeaderViewModel>> Handle (GetJobApplicationHeaderQuery request, CancellationToken cancellationToken)
        {
            var header = await context.JobApplications
                .AsNoTracking()
                .Where(j => j.Id == request.JobId)
                .Select(j => new JobApplicationHeaderViewModel(j.CompanyName, j.Position))
                .FirstOrDefaultAsync(cancellationToken);

            return header == null
                ? Result<JobApplicationHeaderViewModel>.Failure("Job not found")
                : Result<JobApplicationHeaderViewModel>.Success(header);
        }
    }
}
