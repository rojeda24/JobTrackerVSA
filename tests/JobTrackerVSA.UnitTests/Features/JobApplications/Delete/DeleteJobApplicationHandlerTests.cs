using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.Delete;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.Delete
{
    public class DeleteJobApplicationHandlerTests
    {
        [Fact]
        public async Task Handle_Should_DeleteApplication_When_Found()
        {
            // Arrange
            var userId = "user-123";
            using var context = TestDbContextFactory.Create(userId);

            var app = new JobApplication { CompanyName = "To Delete", Position = "Dev", UserId = userId };
            context.JobApplications.Add(app);
            await context.SaveChangesAsync();

            var handler = new DeleteJobApplicationHandler(context);
            var command = new DeleteJobApplicationCommand(app.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            (await context.JobApplications.AnyAsync(a => a.Id == app.Id)).Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NotFound()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new DeleteJobApplicationHandler(context);
            var command = new DeleteJobApplicationCommand(Guid.NewGuid());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }
    }
}
