using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Features.JobApplications.Add;
using JobTrackerVSA.Web.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.Add
{
    public class AddJobApplicationHandlerTests
    {
        [Fact]
        public async Task Handle_Should_AddApplication_When_RequestIsValid()
        {
            // Arrange
            var userId = "user-123";
            using var context = TestDbContextFactory.Create(userId);
            
            var mockUserService = Substitute.For<ICurrentUserService>();
            mockUserService.UserId.Returns(userId);

            var handler = new AddJobApplicationHandler(context, mockUserService);
            
            var command = new AddJobApplicationCommand
            {
                CompanyName = "Google",
                Position = "Senior Engineer",
                JobDescriptionUrl = "https://google.com/jobs/123",
                AppliedAt = DateTime.UtcNow,
                Notes = "Referral from John"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            var appInDb = await context.JobApplications.FindAsync(result.Value);
            appInDb.Should().NotBeNull();
            appInDb!.CompanyName.Should().Be("Google");
            appInDb!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_UserIsNotLoggedIn()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            
            var mockUserService = Substitute.For<ICurrentUserService>();
            mockUserService.UserId.Returns((string?)null); // Simulate not logged in

            var handler = new AddJobApplicationHandler(context, mockUserService);
            
            var command = new AddJobApplicationCommand 
            {
                CompanyName = "Google",
                Position = "Senior Engineer",
                JobDescriptionUrl = "https://google.com/jobs/123",
                AppliedAt = DateTime.UtcNow,
                Notes = "Referral from John"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
