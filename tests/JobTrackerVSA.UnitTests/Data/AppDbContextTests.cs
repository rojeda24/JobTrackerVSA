using FluentAssertions;
using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobTrackerVSA.UnitTests.Data;

public class AppDbContextTests
{
    // Dummy CurrentUserService needed to instantiate AppDbContext
    private class DummyCurrentUserService : ICurrentUserService
    {
        public string UserId => "test-user-id";
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureJobApplicationPropertiesCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var currentUserService = new DummyCurrentUserService();

        using var context = new AppDbContext(options, currentUserService);

        // Act
        // Accessing the Model property compiles the model and runs OnModelCreating
        var model = context.Model;

        // Assert
        var jobAppEntity = model.FindEntityType(typeof(JobApplication));
        jobAppEntity.Should().NotBeNull();

        // Check UserId length
        var userIdProperty = jobAppEntity!.FindProperty(nameof(JobApplication.UserId));
        userIdProperty.Should().NotBeNull();
        userIdProperty!.GetMaxLength().Should().Be(255);

        // Check CompanyName length
        var companyNameProperty = jobAppEntity.FindProperty(nameof(JobApplication.CompanyName));
        companyNameProperty.Should().NotBeNull();
        companyNameProperty!.GetMaxLength().Should().Be(150);

        // Check Position length
        var positionProperty = jobAppEntity.FindProperty(nameof(JobApplication.Position));
        positionProperty.Should().NotBeNull();
        positionProperty!.GetMaxLength().Should().Be(150);

        // Check JobDescriptionUrl length and nullability
        var jobDescriptionUrlProperty = jobAppEntity.FindProperty(nameof(JobApplication.JobDescriptionUrl));
        jobDescriptionUrlProperty.Should().NotBeNull();
        jobDescriptionUrlProperty!.GetMaxLength().Should().Be(2048);
        jobDescriptionUrlProperty.IsNullable.Should().BeTrue(); // Verify it allows nulls
        
        // Check Notes length for JobApplication
        var jobAppNotesProperty = jobAppEntity.FindProperty(nameof(JobApplication.Notes));
        jobAppNotesProperty.Should().NotBeNull();
        jobAppNotesProperty!.GetMaxLength().Should().Be(4000);

        // Check CoverLetter length for JobApplication
        var jobAppCoverLetterProperty = jobAppEntity.FindProperty(nameof(JobApplication.CoverLetter));
        jobAppCoverLetterProperty.Should().NotBeNull();
        jobAppCoverLetterProperty!.GetMaxLength().Should().Be(4000);
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureInterviewRelationshipsCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var currentUserService = new DummyCurrentUserService();

        using var context = new AppDbContext(options, currentUserService);

        // Act
        var model = context.Model;

        // Assert
        var interviewEntity = model.FindEntityType(typeof(Interview));
        interviewEntity.Should().NotBeNull();

        // Check Relationship from Interview to JobApplication
        var foreignKey = interviewEntity!.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(JobApplication));

        foreignKey.Should().NotBeNull();
        foreignKey!.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
        
        // Check Notes length for Interview
        var interviewNotesProperty = interviewEntity.FindProperty(nameof(Interview.Notes));
        interviewNotesProperty.Should().NotBeNull();
        interviewNotesProperty!.GetMaxLength().Should().Be(5120);
    }

    [Fact]
    public void OnModelCreating_ShouldConfigureJobApplicationAnalyticsView()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var currentUserService = new DummyCurrentUserService();

        using var context = new AppDbContext(options, currentUserService);

        // Act
        var model = context.Model;

        // Assert
        var analyticsEntity = model.FindEntityType(typeof(JobApplicationAnalytics));
        analyticsEntity.Should().NotBeNull();

        // Check that it's mapped to the correct view name
        analyticsEntity!.GetViewName().Should().Be("vw_JobApplicationAnalytics");
    }
}
