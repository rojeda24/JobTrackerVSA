using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.ViewResume;
using JobTrackerVSA.Web.Infrastructure.Storage;
using NSubstitute;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.ViewResume;

public class GetResumeRedirectHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnSecureUrl_When_JobApplicationExistsAndHasResume()
    {
        // Arrange
        var userId = "user-123";
        using var context = TestDbContextFactory.Create(userId);

        var jobApp = new JobApplication
        {
            CompanyName = "Microsoft",
            Position = "Developer",
            UserId = userId,
            ResumeUrl = "https://azure.com/resume.pdf"
        };

        context.JobApplications.Add(jobApp);
        await context.SaveChangesAsync();

        var mockStorageService = Substitute.For<IResumeStorageService>();
        mockStorageService.GetSecureResumeUrl("https://azure.com/resume.pdf", 1).Returns("https://azure.com/resume.pdf?sastoken");

        var handler = new GetResumeRedirectHandler(context, mockStorageService);
        var query = new GetResumeRedirectQuery(jobApp.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("https://azure.com/resume.pdf?sastoken");
        mockStorageService.Received(1).GetSecureResumeUrl("https://azure.com/resume.pdf", 1);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_JobApplicationDoesNotExist()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var mockStorageService = Substitute.For<IResumeStorageService>();

        var handler = new GetResumeRedirectHandler(context, mockStorageService);
        var query = new GetResumeRedirectQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found or unauthorized");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_JobApplicationBelongsToAnotherUser()
    {
        // Arrange
        var currentUser = "user-A";
        var otherUser = "user-B";
        using var context = TestDbContextFactory.Create(currentUser);

        var jobApp = new JobApplication
        {
            CompanyName = "Other Corp",
            Position = "Dev",
            UserId = otherUser,
            ResumeUrl = "https://azure.com/resume.pdf"
        };
        context.JobApplications.Add(jobApp);
        await context.SaveChangesAsync();

        var mockStorageService = Substitute.For<IResumeStorageService>();
        var handler = new GetResumeRedirectHandler(context, mockStorageService);
        var query = new GetResumeRedirectQuery(jobApp.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found or unauthorized");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ResumeUrlIsNull()
    {
        // Arrange
        var userId = "user-123";
        using var context = TestDbContextFactory.Create(userId);

        var jobApp = new JobApplication
        {
            CompanyName = "Microsoft",
            Position = "Developer",
            UserId = userId,
            ResumeUrl = null
        };

        context.JobApplications.Add(jobApp);
        await context.SaveChangesAsync();

        var mockStorageService = Substitute.For<IResumeStorageService>();

        var handler = new GetResumeRedirectHandler(context, mockStorageService);
        var query = new GetResumeRedirectQuery(jobApp.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found or unauthorized");
        mockStorageService.DidNotReceive().GetSecureResumeUrl(Arg.Any<string>(), Arg.Any<int>());
    }
}
