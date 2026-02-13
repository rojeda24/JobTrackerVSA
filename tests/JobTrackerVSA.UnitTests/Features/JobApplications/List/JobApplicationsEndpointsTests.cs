using FluentAssertions;
using JobTrackerVSA.Web.Features.JobApplications.List;
using JobTrackerVSA.Web.Infrastructure.Shared;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.List;

public class JobApplicationsEndpointsTests
{
    private readonly IMediator _mediator;

    public JobApplicationsEndpointsTests()
    {
        _mediator = Substitute.For<IMediator>();
    }

    [Fact]
    public async Task GetJobApplications_ShouldReturnOk_WhenQuerySucceeds()
    {
        // Arrange
        var pagedList = new PagedList<JobApplicationSummaryViewModel>(
            new List<JobApplicationSummaryViewModel>(), 1, 10, 0
        );

        var successResult = Result<PagedList<JobApplicationSummaryViewModel>>.Success(pagedList);

        _mediator.Send(Arg.Any<GetJobApplicationsQuery>()).Returns(successResult);

        // Act
        var result = await JobApplicationsEndpoints.HandleGetJobApplications(1, _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<PagedList<JobApplicationSummaryViewModel>>>().Subject;
        okResult.Value.Should().Be(pagedList);
    }

    [Fact]
    public async Task GetJobApplications_ShouldReturnBadRequest_WhenQueryFails()
    {
        // Arrange
        var failureResult = Result<PagedList<JobApplicationSummaryViewModel>>.Failure("Database error");
        _mediator.Send(Arg.Any<GetJobApplicationsQuery>()).Returns(failureResult);

        // Act
        var result = await JobApplicationsEndpoints.HandleGetJobApplications(1, _mediator);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequest<string>>().Subject;
        badRequestResult.Value.Should().Be("Database error");
    }
    
    [Fact]
    public async Task GetJobApplications_ShouldUseDefaultPage_WhenPageIsNull()
    {
        // Arrange
        var pagedList = new PagedList<JobApplicationSummaryViewModel>(
            new List<JobApplicationSummaryViewModel>(), 1, 10, 0
        );
        _mediator.Send(Arg.Any<GetJobApplicationsQuery>()).Returns(Result<PagedList<JobApplicationSummaryViewModel>>.Success(pagedList));

        // Act
        await JobApplicationsEndpoints.HandleGetJobApplications(null, _mediator);

        // Assert
        // Verify that the query was sent with Page = 1 (default from record constructor)
        await _mediator.Received().Send(Arg.Is<GetJobApplicationsQuery>(q => q.Page == 1));
    }
}
