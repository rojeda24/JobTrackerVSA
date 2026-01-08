using JobTrackerVSA.Web.Data;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public class EditJobApplicationHandler (AppDbContext context): IRequestHandler<EditJobApplicationCommand>
    {
        public async Task Handle(EditJobApplicationCommand command, CancellationToken cancellationToken) 
        {
            var app = await context.JobApplications
                .FindAsync([command.Id], cancellationToken);

            if (app == null) return;

            app.CompanyName = command.CompanyName;
            app.Position = command.Position;
            app.JobDescriptionUrl = command.JobDescriptionUrl;
            app.AppliedAt = command.AppliedAt;
            app.Status = command.Status;
            app.Notes = command.Notes;

            await context.SaveChangesAsync(cancellationToken); //TODO: Aqui estamos asumiendo que todo se grabo bien, implementar Result Pattern
        }
    }
}
