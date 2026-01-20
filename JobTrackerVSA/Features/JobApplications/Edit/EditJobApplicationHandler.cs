using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.Edit
{
    public class EditJobApplicationHandler (AppDbContext context): IRequestHandler<EditJobApplicationCommand, Result>
    {
        public async Task<Result> Handle(EditJobApplicationCommand command, CancellationToken cancellationToken) 
        {
            var app = await context.JobApplications
                .FindAsync([command.Id], cancellationToken);

            if (app == null) 
                return Result.Failure($"No Job Application found with ID {command.Id}");

            app.CompanyName = command.CompanyName;
            app.Position = command.Position;
            app.JobDescriptionUrl = command.JobDescriptionUrl;
            app.AppliedAt = DateTime.SpecifyKind(command.AppliedAt, DateTimeKind.Utc);
            app.Status = command.Status;
            app.Notes = command.Notes;

            //try
            //{
                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            //}
            //catch (DbUpdateException)
            //{
            //    //TODO: LOG EXCEPTION
            //    return Result.Failure($"Unexpected error when trying to save Job Application with ID {command.Id} in database");
            //}          
        }
    }
}
