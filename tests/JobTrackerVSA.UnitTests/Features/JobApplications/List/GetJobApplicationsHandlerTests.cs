using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.JobApplications.List;

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
            result.Value.Items.Should().HaveCount(2); // Should only see the 2 apps for user-A
            result.Value.Items.Should().OnlyContain(x => x.CompanyName.StartsWith("My Company"));
            result.Value.TotalCount.Should().Be(2);
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
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_Should_ReturnCorrectPage_When_PaginationIsApplied()
        {
            // Arrange
            var currentUser = "user-P";
            using var context = TestDbContextFactory.Create(currentUser);

            // Seed 25 applications to test pagination (Default PageSize is 10)
            for (int i = 1; i <= 25; i++)
            {
                context.JobApplications.Add(new JobApplication
                {
                    CompanyName = $"Company {i:D2}",
                    Position = "Dev",
                    UserId = currentUser,
                    AppliedAt = DateTime.UtcNow.AddMinutes(i)
                });
            }
            await context.SaveChangesAsync();
            
            var handler = new GetJobApplicationsHandler(context);
            
            // Request Page 2. Since PageSize is 10, this should return items 11-20 (sorted descending by date)
            // Total 25 items. 
            // Page 1: 25..16
            // Page 2: 15..06
            // Page 3: 05..01
            var query = new GetJobApplicationsQuery(Page: 2);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(10); // Default PageSize
            result.Value.Page.Should().Be(2);
            result.Value.PageSize.Should().Be(10); // Verified Default
            result.Value.TotalCount.Should().Be(25);
            result.Value.TotalPages.Should().Be(3);
            result.Value.HasNextPage.Should().BeTrue();
            result.Value.HasPreviousPage.Should().BeTrue();
            
            // Check ordering (OrderByDescending AppliedAt)
            // Company 25 is most recent.
            // Page 2 should contain Company 15 down to Company 06.
            result.Value.Items.First().CompanyName.Should().Be("Company 15");
            result.Value.Items.Last().CompanyName.Should().Be("Company 06");
        }

        [Fact]
        public async Task Handle_Should_FilterBySearchTerm()
        {
            // Arrange
            var currentUser = "user-search";
            using var context = TestDbContextFactory.Create(currentUser);

            context.JobApplications.AddRange(
                new JobApplication { CompanyName = "Google", Position = "Dev", UserId = currentUser, AppliedAt = DateTime.UtcNow },
                new JobApplication { CompanyName = "Microsoft", Position = "Lead", UserId = currentUser, AppliedAt = DateTime.UtcNow },
                new JobApplication { CompanyName = "Amazon", Position = "Architect", UserId = currentUser, AppliedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
            
            var handler = new GetJobApplicationsHandler(context);
            var companyQuery = new GetJobApplicationsQuery(SearchTerm: "soft");
            var positionQuery = new GetJobApplicationsQuery(SearchTerm: "rchitec");

            // Act
            var companyResult = await handler.Handle(companyQuery, CancellationToken.None);
            var positionResult = await handler.Handle(positionQuery, CancellationToken.None);

            // Assert
            companyResult.IsSuccess.Should().BeTrue();
            companyResult.Value.Items.Should().HaveCount(1);
            companyResult.Value.Items.First().CompanyName.Should().Be("Microsoft");

            positionResult.IsSuccess.Should().BeTrue();
            positionResult.Value.Items.Should().HaveCount(1);
            positionResult.Value.Items.First().CompanyName.Should().Be("Amazon");
        }
    }
}
