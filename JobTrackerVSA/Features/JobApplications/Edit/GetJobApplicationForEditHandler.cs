using JobTrackerVSA.Web.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public class GetJobApplicationForEditHandler (AppDbContext context): IRequestHandler<GetJobApplicationForEditQuery, EditModel.InputModel?> //TODO PREGUNTAR PORQUE NULLEABLE
    {
        public async Task<EditModel.InputModel?> Handle(GetJobApplicationForEditQuery request, CancellationToken cancellationToken)
        {
            var app = await context.JobApplications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (app == null) return null;

            return new EditModel.InputModel
            {
                Id = app.Id,
                CompanyName = app.CompanyName,
                Position = app.Position,
                JobDescriptionUrl = app.JobDescriptionUrl,
                AppliedAt = app.AppliedAt,
                Status = app.Status,
                Notes = app.Notes
            };
        }
    }
}
