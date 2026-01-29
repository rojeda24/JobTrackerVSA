using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Interviews.Edit;

namespace JobTrackerVSA.UnitTests.Features.Interviews.Edit
{
    public class EditInterviewHandlerTests
    {
        [Fact]
        public async Task Handle_Should_UpdateInterview_When_Found()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            var interview = new Interview
            {
                JobApplicationId = Guid.NewGuid(),
                ScheduledAt = DateTime.UtcNow,
                Type = Interview.InterviewType.General,
                Notes = "Old notes"
            };
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            var handler = new EditInterviewHandler(context);
            var newDate = DateTime.UtcNow.AddDays(5);
            var command = new EditInterviewCommand(
                interview.Id,
                interview.JobApplicationId,
                newDate,
                Interview.InterviewType.HR,
                "New notes"
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            var updatedInterview = await context.Interviews.FindAsync(interview.Id);
            updatedInterview!.Type.Should().Be(Interview.InterviewType.HR);
            updatedInterview.Notes.Should().Be("New notes");
            updatedInterview.ScheduledAt.Should().Be(newDate);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_NotFound()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var handler = new EditInterviewHandler(context);
            
            var command = new EditInterviewCommand(
                Guid.NewGuid(), 
                Guid.NewGuid(), 
                DateTime.UtcNow, 
                Interview.InterviewType.General, 
                "Notes"
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }
    }
}
