using FluentAssertions;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.ViewResume;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.ViewResume;

public class ViewResumeEndpointTests
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobApplication> _logger;

    public ViewResumeEndpointTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<JobApplication>>();
    }

    [Fact]
    public async Task HandleGetResumeRedirect_ShouldReturnRedirect_WhenQuerySucceeds()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var secureUrl = "https://azure.com/resume.pdf?sastoken";
        
        var successResult = Result<string>.Success(secureUrl);
        _mediator.Send(
            Arg.Is<GetResumeRedirectQuery>(q => q.JobApplicationId == jobId)
        ).Returns(successResult);

        // Act
        var result = await ViewResumeEndpoint.HandleGetResumeRedirect(jobId, _mediator, _logger);

        // Assert
        var redirectResult = result.Should().BeOfType<RedirectHttpResult>().Subject;
        redirectResult.Url.Should().Be(secureUrl);
        
        // Ensure logger was NOT called with a warning
        _logger.DidNotReceiveWithAnyArgs().LogWarning(default);
    }

    [Fact]
    public async Task HandleGetResumeRedirect_ShouldReturnNotFound_WhenQueryFails()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var errorMessage = "Resume not found or unauthorized.";
        
        var failureResult = Result<string>.Failure(errorMessage);
        _mediator.Send(Arg.Is<GetResumeRedirectQuery>(q => q.JobApplicationId == jobId)).Returns(failureResult);

        // Act
        var result = await ViewResumeEndpoint.HandleGetResumeRedirect(jobId, _mediator, _logger);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFound<string>>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }
}