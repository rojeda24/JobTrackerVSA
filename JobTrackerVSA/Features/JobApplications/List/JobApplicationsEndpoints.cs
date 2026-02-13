using MediatR;
using JobTrackerVSA.Web.Infrastructure.Shared;

namespace JobTrackerVSA.Web.Features.JobApplications.List;

public static class JobApplicationsEndpoints
{
    public static void MapJobApplicationsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/job-applications", async (int? page, IMediator mediator) =>
        {
            var query = page.HasValue ? new GetJobApplicationsQuery(page.Value) : new GetJobApplicationsQuery();
            var result = await mediator.Send(query);

            if (result.IsFailure) return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
        .WithTags("JobApplications")
        .WithSummary("Gets a paginated list of job applications for the current user")
        .Produces<PagedList<JobApplicationSummaryViewModel>>(200)
        .Produces(400)
        .Produces(401)
        .RequireAuthorization();
    }
}

