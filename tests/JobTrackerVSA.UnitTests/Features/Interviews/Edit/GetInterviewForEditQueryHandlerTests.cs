using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Interviews.Edit;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.Interviews.Edit
{
    public class GetInterviewForEditQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_ReturnCommand_When_InterviewExistsAndBelongsToUser()
        {
            // Arrange
            var userId = "user-123";
            using var context = TestDbContextFactory.Create(userId);

            var jobApp = new JobApplication
            {
                CompanyName = "Test Corp",
                Position = "Dev",
                UserId = userId,
                AppliedAt = DateTime.UtcNow
            };
            context.JobApplications.Add(jobApp);

            var interview = new Interview
            {
                JobApplicationId = jobApp.Id,
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                Type = Interview.InterviewType.Technical,
                Notes = "Notes"
            };
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            var handler = new GetInterviewForEditQueryHandler(context);
            var query = new GetInterviewForEditQuery(interview.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(interview.Id);
            result.Value.ScheduledAt.Kind.Should().Be(DateTimeKind.Utc);
            result.Value.Type.Should().Be(Interview.InterviewType.Technical);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_InterviewBelongsToAnotherUser()
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
                AppliedAt = DateTime.UtcNow
            };
            context.JobApplications.Add(jobApp);

            var interview = new Interview
            {
                JobApplicationId = jobApp.Id,
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                Type = Interview.InterviewType.General
            };
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            var handler = new GetInterviewForEditQueryHandler(context);
            var query = new GetInterviewForEditQuery(interview.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_InterviewDoesNotExist()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new GetInterviewForEditQueryHandler(context);
            var query = new GetInterviewForEditQuery(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }
    }
}
