using FluentAssertions;
using JobTrackerVSA.UnitTests.Data;
using JobTrackerVSA.Web.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace JobTrackerVSA.UnitTests.Features.Interviews
{
    public class InterviewStatusInterceptorTests
    {
        [Theory]
        [InlineData(Interview.InterviewType.Technical, JobApplication.ApplicationStatus.TechnicalTest)]
        [InlineData(Interview.InterviewType.Proposal, JobApplication.ApplicationStatus.Offered)]
        [InlineData(Interview.InterviewType.General, JobApplication.ApplicationStatus.Interviewing)]
        [InlineData(Interview.InterviewType.HR, JobApplication.ApplicationStatus.Interviewing)]
        public async Task AddingInterview_UpdatesJobApplicationStatus(
            Interview.InterviewType interviewType,
            JobApplication.ApplicationStatus expectedStatus)
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            var jobApp = new JobApplication
            {
                CompanyName = "Acme",
                Position = "Tester",
                Status = JobApplication.ApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                UserId = "user-123"
            };

            context.JobApplications.Add(jobApp);
            await context.SaveChangesAsync();

            var interview = new Interview
            {
                JobApplicationId = jobApp.Id,
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                Type = interviewType,
                Notes = null
            };

            // Act
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            // Assert
            var updated = await context.JobApplications.FindAsync(jobApp.Id);
            updated.Should().NotBeNull();
            updated!.Status.Should().Be(expectedStatus);
        }

        [Theory]
        [InlineData(Interview.InterviewType.General, Interview.InterviewType.Proposal, JobApplication.ApplicationStatus.Offered)]
        [InlineData(Interview.InterviewType.Technical, Interview.InterviewType.Proposal, JobApplication.ApplicationStatus.Offered)]
        [InlineData(Interview.InterviewType.Technical, Interview.InterviewType.HR, JobApplication.ApplicationStatus.Interviewing)]
        public async Task EditingInterview_ChangesJobApplicationStatus(
            Interview.InterviewType initialType,
            Interview.InterviewType newType,
            JobApplication.ApplicationStatus expectedStatus)
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            var jobApp = new JobApplication
            {
                CompanyName = "Contoso",
                Position = "Developer",
                Status = JobApplication.ApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                UserId = "user-123"
            };

            var interview = new Interview
            {
                JobApplicationId = jobApp.Id,
                ScheduledAt = DateTime.UtcNow,
                Type = initialType
            };

            context.JobApplications.Add(jobApp);
            context.Interviews.Add(interview);
            await context.SaveChangesAsync();

            // sanity: initial status reflects the original interview type
            var initial = await context.JobApplications.FindAsync(jobApp.Id);
            initial!.Status.Should().Be(initialType switch
            {
                Interview.InterviewType.Technical => JobApplication.ApplicationStatus.TechnicalTest,
                Interview.InterviewType.Proposal => JobApplication.ApplicationStatus.Offered,
                _ => JobApplication.ApplicationStatus.Interviewing
            });

            // Act: change interview type
            interview.Type = newType;
            context.Interviews.Update(interview);
            await context.SaveChangesAsync();

            var updated = await context.JobApplications.FindAsync(jobApp.Id);
            updated!.Status.Should().Be(expectedStatus);
        }
    }
}