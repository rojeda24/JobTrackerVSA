using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.List
{
    public class GetJobApplicationsHandler(AppDbContext context): IRequestHandler<GetJobApplicationsQuery, Result<List<JobApplicationSummaryViewModel>>>
    {
        public async Task<Result<List<JobApplicationSummaryViewModel>>> Handle(GetJobApplicationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var apps = await context.JobApplications
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
                return Result<List<JobApplicationSummaryViewModel>>.Success(apps);
            }
            catch (Exception ex) {
                return Result<List<JobApplicationSummaryViewModel>>.Failure($"Unexpected server error when trying to load job applications: {ex}");
            }
        }
    }
}
