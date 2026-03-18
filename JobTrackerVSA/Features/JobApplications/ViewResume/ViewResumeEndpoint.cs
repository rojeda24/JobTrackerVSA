using JobTrackerVSA.Web.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobTrackerVSA.Web.Features.JobApplications.ViewResume;

public static class ViewResumeEndpoint
{
    public static void MapViewResumeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/job-applications/{id:guid}/resume", HandleGetResumeRedirect)
            .WithTags("JobApplications")
            .WithSummary("Redirects to a secure short-lived SAS URL to view the resume")
            .Produces(302)
            .Produces(404)
            .Produces(401)
            .RequireAuthorization();
    }

    internal static async Task<IResult> HandleGetResumeRedirect(Guid id, IMediator mediator, ILogger<JobApplication> logger)
    {
        var result = await mediator.Send(new GetResumeRedirectQuery(id));
        
        if (result.IsFailure)
        {
            logger.LogWarning("Failed resume redirect attempt for JobApplication ID {Id}. Error: {Error}", id, result.Error);
            return Results.NotFound(result.Error);
        }

        return Results.Redirect(result.Value);
    }
}
