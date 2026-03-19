using System.Text;
using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Features.JobApplications.ExportCoverLetters;

public class ExportCoverLettersHandler(AppDbContext db) : IRequestHandler<ExportCoverLettersQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(ExportCoverLettersQuery request, CancellationToken cancellationToken)
    {
        var applications = await db.JobApplications
            .Where(x => !string.IsNullOrEmpty(x.CoverLetter))
            .OrderByDescending(x => x.AppliedAt)
            .Select(x => new { x.AppliedAt, x.CompanyName, x.CoverLetter })
            .ToListAsync(cancellationToken);

        if (applications.Count == 0)
        {
            return Result<byte[]>.Failure("No cover letters found. You haven't added any cover letters to your job applications yet.");
        }

        StringBuilder sb = new();
        foreach (var app in applications)
        {
            sb.AppendLine("---");
            sb.AppendLine($"**Date**: {app.AppliedAt:yyyy-MM-dd}  ");
            sb.AppendLine($"**Company**: {app.CompanyName}  ");
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine(app.CoverLetter);
            sb.AppendLine();
            sb.AppendLine();
        }

        byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
        return Result<byte[]>.Success(fileBytes);
    }
}
