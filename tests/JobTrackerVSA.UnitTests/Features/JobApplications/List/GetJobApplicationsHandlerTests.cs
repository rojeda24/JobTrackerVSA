using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.List;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.List
{
    public class GetJobApplicationsHandlerTests
    {
        [Fact]
        public async Task Handle_Should_ReturnOnlyCurrentUserApplications()
        {
            // Arrange
            var currentUser = "user-A";
            var otherUser = "user-B";
            
            using var context = TestDbContextFactory.Create(currentUser);

            // Seed data for Current User
            context.JobApplications.Add(new JobApplication 
            { 
                CompanyName = "My Company 1", 
                Position = "Dev", 
                UserId = currentUser,
                AppliedAt = DateTime.UtcNow 
            });
            context.JobApplications.Add(new JobApplication 
            { 
                CompanyName = "My Company 2", 
                Position = "Lead", 
                UserId = currentUser,
                AppliedAt = DateTime.UtcNow.AddDays(-1)
            });

            // Seed data for Other User
            context.JobApplications.Add(new JobApplication 
            { 
                CompanyName = "Other Company", 
                Position = "CEO", 
                UserId = otherUser, // Different User ID
                AppliedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var handler = new GetJobApplicationsHandler(context);
            var query = new GetJobApplicationsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2); // Should only see the 2 apps for user-A
            result.Value.Should().OnlyContain(x => x.CompanyName.StartsWith("My Company"));
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_NoAppsExistForUser()
        {
            // Arrange
            using var context = TestDbContextFactory.Create("fresh-user");
            // No data seeded for this user

            var handler = new GetJobApplicationsHandler(context);
            var query = new GetJobApplicationsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}
