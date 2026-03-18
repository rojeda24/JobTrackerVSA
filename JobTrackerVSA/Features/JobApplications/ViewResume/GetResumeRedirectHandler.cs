using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using JobTrackerVSA.Web.Infrastructure.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.ViewResume;

public record GetResumeRedirectQuery(Guid JobApplicationId) : IRequest<Result<string>>;

public class GetResumeRedirectHandler(
    AppDbContext context,
    IResumeStorageService storageService) 
    : IRequestHandler<GetResumeRedirectQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetResumeRedirectQuery request, CancellationToken cancellationToken)
    {
        var application = await context.JobApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.JobApplicationId, cancellationToken);

        if (application == null || string.IsNullOrEmpty(application.ResumeUrl))
        {
            return Result<string>.Failure("Resume not found or unauthorized.");
        }

        var secureUrl = storageService.GetSecureResumeUrl(application.ResumeUrl); // 1 minute expiration

        return Result<string>.Success(secureUrl);
    }
}
