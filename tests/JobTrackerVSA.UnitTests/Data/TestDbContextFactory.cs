using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;

namespace JobTrackerVSA.UnitTests.Data
{
    public static class TestDbContextFactory
    {
        public static AppDbContext Create(string currentUserId = "user-123")
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            var mockUserService = Substitute.For<ICurrentUserService>();
            mockUserService.UserId.Returns(currentUserId);

            var context = new AppDbContext(options, mockUserService);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
