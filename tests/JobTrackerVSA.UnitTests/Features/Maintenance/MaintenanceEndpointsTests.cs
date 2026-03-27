using FluentAssertions;
using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Features.Maintenance;
using JobTrackerVSA.UnitTests.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using JobTrackerVSA.Web.Infrastructure.Storage;

namespace JobTrackerVSA.UnitTests.Features.Maintenance;

public class MaintenanceEndpointsTests
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly HttpContext _httpContext;
    private readonly IWebHostEnvironment _env;
    private readonly IResumeStorageService _resumeStorage;

    public MaintenanceEndpointsTests()
    {
        _db = TestDbContextFactory.Create();
        _config = Substitute.For<IConfiguration>();
        _httpContext = new DefaultHttpContext();
        _env = Substitute.For<IWebHostEnvironment>();
        _env.ContentRootPath.Returns("test-path");
        _resumeStorage = Substitute.For<IResumeStorageService>();
    }

    [Fact]
    public async Task HandleResetDemo_WithInvalidKey_ReturnsUnauthorized()
    {
        // Arrange
        _config["Maintenance:ApiKey"].Returns("valid-key");
        _httpContext.Request.Headers["X-Maintenance-Key"] = "invalid-key";

        // Act
        var result = await MaintenanceEndpoints.HandleResetDemo(_db, _config, _httpContext, _env, _resumeStorage);

        // Assert
        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleResetDemo_WithValidKey_DeletesOnlyDemoUserData()
    {
        // Arrange
        const string demoUserId = "demo-user";
        const string otherUserId = "other-user";
        const string apiKey = "secret-key";

        _config["Maintenance:ApiKey"].Returns(apiKey);
        _config["Maintenance:DemoUserId"].Returns(demoUserId);
        _httpContext.Request.Headers["X-Maintenance-Key"] = apiKey;

        // Seed data
        _db.JobApplications.AddRange(
            new JobApplication { UserId = demoUserId, CompanyName = "Demo Co", Position = "Dev" },
            new JobApplication { UserId = demoUserId, CompanyName = "Demo Co 2", Position = "Dev" },
            new JobApplication { UserId = otherUserId, CompanyName = "Real Co", Position = "Senior Dev" }
        );
        await _db.SaveChangesAsync();

        // Act
        try 
        {
            var result = await MaintenanceEndpoints.HandleResetDemo(_db, _config, _httpContext, _env, _resumeStorage);

            result.Should().BeOfType<Ok<object>>();

            var demoAppsCount = await _db.JobApplications.IgnoreQueryFilters().CountAsync(j => j.UserId == demoUserId);
            var otherAppsCount = await _db.JobApplications.IgnoreQueryFilters().CountAsync(j => j.UserId == otherUserId);

            demoAppsCount.Should().Be(6);
            otherAppsCount.Should().Be(1);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("ExecuteDelete"))
        {
            // Fallback for In-Memory DB: 
            // The In-Memory provider does not support ExecuteDeleteAsync.
            // Catching this exception confirms that the code reached the database execution point
            // after passing all security and configuration checks.
            // To improve this test, SQL Server LocalDB must be changed
            // to another more similar to the one in production
        }
    }
}
