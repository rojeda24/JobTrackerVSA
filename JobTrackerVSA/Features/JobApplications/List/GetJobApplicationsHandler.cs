using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.List;

public class GetJobApplicationsHandler(AppDbContext context): IRequestHandler<GetJobApplicationsQuery, Result<PagedList<JobApplicationSummaryViewModel>>>
{
    private const int PageSize = 10;

    public async Task<Result<PagedList<JobApplicationSummaryViewModel>>> Handle(GetJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = context.JobApplications.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => x.CompanyName.Contains(request.SearchTerm) || x.Position.Contains(request.SearchTerm));
        }

        var projectedQuery = query

            .OrderByDescending(x => x.AppliedAt)
            .Select(x => new JobApplicationSummaryViewModel
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                Position = x.Position,
                JobDescriptionUrl = x.JobDescriptionUrl,
                NextInterviewAt = x.Interviews
                    .Where(i => i.ScheduledAt > DateTime.UtcNow)
                    .OrderBy(i => i.ScheduledAt)
                    .Select(i => (DateTime?)i.ScheduledAt)
                    .FirstOrDefault(),
                Status = x.Status,
                Notes = x.Notes,
                ResumeUrl = x.ResumeUrl
            });

        var pagedApps = await projectedQuery.ToPagedListAsync(request.Page, PageSize, cancellationToken);
        return Result<PagedList<JobApplicationSummaryViewModel>>.Success(pagedApps);
    }

}
