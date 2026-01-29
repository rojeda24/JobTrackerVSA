using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.Edit;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.Edit
{
    public class EditJobApplicationHandlerTests
    {
        [Fact]
        public async Task Handle_Should_UpdateApplication_When_Found()
        {
            // Arrange
            var userId = "user-123";
            using var context = TestDbContextFactory.Create(userId);

            var existingApp = new JobApplication
            {
                CompanyName = "Old Corp",
                Position = "Junior",
                UserId = userId,
                Status = JobApplication.ApplicationStatus.Applied
            };
            context.JobApplications.Add(existingApp);
            await context.SaveChangesAsync();

            var handler = new EditJobApplicationHandler(context);

            var now = DateTime.UtcNow;

            var command = new EditJobApplicationCommand(
                existingApp.Id,
                "New Corp",
                "Senior",
                "http://newUrl",
                now,
                JobApplication.ApplicationStatus.Interviewing,
                "Updated notes"
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var updatedApp = await context.JobApplications.FindAsync(existingApp.Id);
            updatedApp.Should().NotBeNull();
            updatedApp!.CompanyName.Should().Be("New Corp");
            updatedApp.Position.Should().Be("Senior");
            updatedApp.JobDescriptionUrl.Should().Be("http://newUrl");
            updatedApp.AppliedAt.Should().Be(now);
            updatedApp.Status.Should().Be(JobApplication.ApplicationStatus.Interviewing);
            updatedApp.Notes.Should().Be("Updated notes");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NotFound()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new EditJobApplicationHandler(context);

            var command = new EditJobApplicationCommand(
                Guid.NewGuid(), // Random ID
                "Company", 
                "Position", 
                null, 
                DateTime.UtcNow, 
                JobApplication.ApplicationStatus.Applied, 
                null
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("No Job Application found");
        }
    }
}
