using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackerVSA.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalyticsViewWithStatusName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW [dbo].[vw_JobApplicationAnalytics] AS
                SELECT
                    ja.Id AS JobApplicationId,
                    ja.UserId,
                    ja.CompanyName,
                    ja.AppliedAt,
                    CAST(ja.AppliedAt AS date) AS AppliedDate,
                    DATEPART(weekday, ja.AppliedAt) AS AppliedDayOfWeek,
                    DATENAME(weekday, ja.AppliedAt) AS AppliedDayOfWeekName,
                    ja.Status AS ApplicationStatus,
                    
                    -- Map enum int values to string names directly in SQL
                    CASE ja.Status
                        WHEN 0 THEN 'Applied'
                        WHEN 1 THEN 'Interviewing'
                        WHEN 2 THEN 'TechnicalTest'
                        WHEN 3 THEN 'Offered'
                        WHEN 4 THEN 'Rejected'
                        WHEN 5 THEN 'Accepted'
                        ELSE 'Unknown'
                    END AS ApplicationStatusName,

                    COUNT(i.Id) AS InterviewCount,
                    CASE WHEN COUNT(i.Id) > 0 THEN 1 ELSE 0 END AS HasInterview

                FROM dbo.JobApplications AS ja
                LEFT JOIN dbo.Interviews AS i
                    ON i.JobApplicationId = ja.Id
                GROUP BY
                    ja.Id,
                    ja.UserId,
                    ja.CompanyName,
                    ja.AppliedAt,
                    ja.Status;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to previous view definition (without ApplicationStatusName)
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW [dbo].[vw_JobApplicationAnalytics] AS
                SELECT
                    ja.Id AS JobApplicationId,
                    ja.UserId,
                    ja.CompanyName,
                    ja.AppliedAt,
                    CAST(ja.AppliedAt AS date) AS AppliedDate,
                    DATEPART(weekday, ja.AppliedAt) AS AppliedDayOfWeek,
                    DATENAME(weekday, ja.AppliedAt) AS AppliedDayOfWeekName,
                    ja.Status AS ApplicationStatus,
                    
                    COUNT(i.Id) AS InterviewCount,
                    CASE WHEN COUNT(i.Id) > 0 THEN 1 ELSE 0 END AS HasInterview

                FROM dbo.JobApplications AS ja
                LEFT JOIN dbo.Interviews AS i
                    ON i.JobApplicationId = ja.Id
                GROUP BY
                    ja.Id,
                    ja.UserId,
                    ja.CompanyName,
                    ja.AppliedAt,
                    ja.Status;
            ");
        }
    }
}
