using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.GetDetails;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.GetDetails
{
    public class GetJobApplicationHeaderHandlerTests
    {
        [Fact]
        public async Task Handle_Should_ReturnHeader_When_JobExistsAndBelongsToUser()
        {
            // Arrange
            var userId = "user-123";
            using var context = TestDbContextFactory.Create(userId);

            var jobApp = new JobApplication
            {
                CompanyName = "Target Corp",
                Position = "Manager",
                UserId = userId,
                AppliedAt = DateTime.UtcNow
            };
            context.JobApplications.Add(jobApp);
            await context.SaveChangesAsync();

            var handler = new GetJobApplicationHeaderHandler(context);
            var query = new GetJobApplicationHeaderQuery(jobApp.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CompanyName.Should().Be("Target Corp");
            result.Value.Position.Should().Be("Manager");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_JobBelongsToAnotherUser()
        {
            // Arrange
            var currentUser = "user-A";
            var otherUser = "user-B";
            
            using var context = TestDbContextFactory.Create(currentUser);

            var otherUserJob = new JobApplication
            {
                CompanyName = "Secret Corp",
                Position = "Spy",
                UserId = otherUser,
                AppliedAt = DateTime.UtcNow
            };
            context.JobApplications.Add(otherUserJob);
            await context.SaveChangesAsync();

            var handler = new GetJobApplicationHeaderHandler(context);
            // User A tries to access User B's job
            var query = new GetJobApplicationHeaderQuery(otherUserJob.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found"); // Should behave as if it doesn't exist
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_JobDoesNotExist()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new GetJobApplicationHeaderHandler(context);
            var query = new GetJobApplicationHeaderQuery(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }
    }
}
