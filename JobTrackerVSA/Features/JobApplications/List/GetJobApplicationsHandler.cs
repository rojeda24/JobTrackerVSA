using JobTrackerVSA.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
    public class GetJobApplicationsHandler(AppDbContext context): IRequestHandler<GetJobApplicationsQuery, List<JobApplicationSummaryViewModel>>
    {
        public async Task<List<JobApplicationSummaryViewModel>> Handle(GetJobApplicationsQuery request, CancellationToken cancellationToken)
        {
            return await context.JobApplications
                .AsNoTracking()
                .OrderByDescending(x => x.AppliedAt)
                .Select(x => new JobApplicationSummaryViewModel
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    Position = x.Position,
                    JobDescriptionUrl = x.JobDescriptionUrl,
                    AppliedAt = x.AppliedAt,
                    Status = x.Status,
                    Notes = x.Notes
                })
                .ToListAsync(cancellationToken);
        }
    }
}
