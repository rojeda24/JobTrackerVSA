using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Interviews.Delete;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.Interviews.Delete
{
    public class DeleteInterviewHandlerTests
    {
        [Fact]
        public async Task Handle_Should_DeleteInterview_When_Found()
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

            var interview = new Interview
            {
                JobApplicationId = jobApp.Id,
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                Type = Interview.InterviewType.Technical,
                Notes = "Prepare algorithms"
            };
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            var handler = new DeleteInterviewHandler(context);
            var command = new DeleteInterviewCommand(interview.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            var interviewInDb = await context.Interviews.FindAsync(interview.Id);
            interviewInDb.Should().BeNull();
            
            var jobInDb = await context.JobApplications.FindAsync(jobApp.Id);
            jobInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NotFound()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new DeleteInterviewHandler(context);
            var command = new DeleteInterviewCommand(Guid.NewGuid());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }
    }
}
