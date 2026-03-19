using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;

namespace JobTrackerVSA.Web.Features.JobApplications.ExportCoverLetters;

public record ExportCoverLettersQuery : IRequest<Result<byte[]>>;
