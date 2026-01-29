using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.Edit;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.Edit
{
    public class GetJobApplicationForEditHandlerTests
    {
        [Fact]
        public async Task Handle_Should_ReturnModel_When_JobExistsAndBelongsToUser()
        {
            // Arrange
            var userId = "user-123";
            using var context = TestDbContextFactory.Create(userId);

            var jobApp = new JobApplication
            {
                CompanyName = "Google",
                Position = "Developer",
                UserId = userId,
                AppliedAt = DateTime.UtcNow,
                Notes = "Some notes",
                Status = JobApplication.ApplicationStatus.Applied
            };
            
            var interview = new Interview
            {
                JobApplicationId = jobApp.Id,
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                Type = Interview.InterviewType.Technical,
                Notes = "Tech notes"
            };
            
            context.JobApplications.Add(jobApp);
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            var handler = new GetJobApplicationForEditHandler(context);
            var query = new GetJobApplicationForEditQuery(jobApp.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(jobApp.Id);
            result.Value.CompanyName.Should().Be("Google");
            result.Value.Interviews.Should().HaveCount(1);
            result.Value.Interviews.First().Id.Should().Be(interview.Id);
            result.Value.Interviews.First().ScheduledAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_JobBelongsToAnotherUser()
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
            await context.SaveChangesAsync();

            var handler = new GetJobApplicationForEditHandler(context);
            var query = new GetJobApplicationForEditQuery(jobApp.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("No Job Application found");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_JobDoesNotExist()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new GetJobApplicationForEditHandler(context);
            var query = new GetJobApplicationForEditQuery(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("No Job Application found");
        }
    }
}
