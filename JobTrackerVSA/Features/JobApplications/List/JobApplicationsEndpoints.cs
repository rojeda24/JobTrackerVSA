using MediatR;
using JobTrackerVSA.Web.Infrastructure.Shared;

namespace JobTrackerVSA.Web.Features.JobApplications.List;


/// <summary>
/// Showcase of API Endpoint not currently being used. Could be used if SPA like React is implemented later.
/// </summary>
public static class JobApplicationsEndpoints
{
    public static void MapJobApplicationsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/job-applications", HandleGetJobApplications)
        .WithTags("JobApplications")
        .WithSummary("Gets a paginated list of job applications for the current user")
        .Produces<PagedList<JobApplicationSummaryViewModel>>(200)
        .Produces(400)
        .Produces(401)
        .RequireAuthorization();
    }

    internal static async Task<IResult> HandleGetJobApplications(
        IMediator mediator,
        int? page,
        string? searchTerm)
    {
        var query = new GetJobApplicationsQuery(page ?? 1, searchTerm);
        var result = await mediator.Send(query);

        if (result.IsFailure) return Results.BadRequest(result.Error);

        return Results.Ok(result.Value);
    }
}

