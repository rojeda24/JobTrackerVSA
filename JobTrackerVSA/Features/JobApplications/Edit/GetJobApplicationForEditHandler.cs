using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public class GetJobApplicationForEditHandler (AppDbContext context): IRequestHandler<GetJobApplicationForEditQuery, Result<EditModel.InputModel>>
    {
        public async Task<Result<EditModel.InputModel>> Handle(GetJobApplicationForEditQuery request, CancellationToken cancellationToken)
        {
            var app = await context.JobApplications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (app == null)
                return Result<EditModel.InputModel>.Failure($"No Job Application found with ID {request.Id}");

            var model = new EditModel.InputModel
            {
                Id = app.Id,
                CompanyName = app.CompanyName,
                Position = app.Position,
                JobDescriptionUrl = app.JobDescriptionUrl,
                AppliedAt = app.AppliedAt,
                Status = app.Status,
                Notes = app.Notes
            };
            return Result<EditModel.InputModel>.Success(model);

        }
    }
}
