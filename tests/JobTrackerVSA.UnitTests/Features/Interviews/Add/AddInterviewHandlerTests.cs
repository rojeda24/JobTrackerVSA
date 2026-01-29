using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Interviews.Add;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.UnitTests.Features.Interviews.Add
{
    public class AddInterviewHandlerTests
    {
        [Fact]
        public async Task Handle_Should_AddInterview_When_JobApplicationExists()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            
            var jobApp = new JobApplication
            {
                CompanyName = "Test Corp",
                Position = "Developer",
                Status = JobApplication.ApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                UserId = "user-123"
            };
            context.JobApplications.Add(jobApp);
            await context.SaveChangesAsync();

            var handler = new AddInterviewHandler(context);
            var command = new AddInterviewCommand(
                jobApp.Id,
                DateTime.UtcNow.AddDays(2),
                Interview.InterviewType.Technical,
                "Study validation"
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            var interviewInDb = await context.Interviews.FirstOrDefaultAsync(i => i.Id == result.Value);
            interviewInDb.Should().NotBeNull();
            interviewInDb.JobApplicationId.Should().Be(jobApp.Id);
            interviewInDb.Type.Should().Be(Interview.InterviewType.Technical);
            interviewInDb.Notes.Should().Be("Study validation");
            interviewInDb.ScheduledAt.Should().Be(command.ScheduledAt);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_JobApplicationDoesNotExist()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            // No JobApplications seeded

            var handler = new AddInterviewHandler(context);
            var command = new AddInterviewCommand(
                Guid.NewGuid(), // Non-existent ID
                DateTime.UtcNow,
                Interview.InterviewType.General,
                null
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
            
            // Verify nothing was saved
            (await context.Interviews.AnyAsync()).Should().BeFalse();
        }
    }
}
