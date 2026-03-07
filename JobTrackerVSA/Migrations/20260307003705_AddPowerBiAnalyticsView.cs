using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackerVSA.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPowerBiAnalyticsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Indexes to support Power BI filters and JOINS at scale
            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_UserId",
                table: "JobApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_AppliedAt",
                table: "JobApplications",
                column: "AppliedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CompanyName",
                table: "JobApplications",
                column: "CompanyName");

            // Analytical view for Power BI / reporting
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[dbo].[vw_JobApplicationAnalytics]', N'V') IS NOT NULL
                    DROP VIEW [dbo].[vw_JobApplicationAnalytics];

                CREATE VIEW [dbo].[vw_JobApplicationAnalytics] AS
                SELECT
                    ja.Id AS JobApplicationId,
                    ja.UserId,
                    ja.CompanyName,
                    ja.Position,
                    ja.AppliedAt,
                    CAST(ja.AppliedAt AS date) AS AppliedDate,
                    DATEPART(weekday, ja.AppliedAt) AS AppliedDayOfWeek,
                    DATENAME(weekday, ja.AppliedAt) AS AppliedDayOfWeekName,
                    ja.Status AS ApplicationStatus,
                    ja.Notes AS ApplicationNotes,
                    COUNT(i.Id) AS InterviewCount,
                    CASE WHEN COUNT(i.Id) > 0 THEN 1 ELSE 0 END AS HasInterview,
                    MIN(i.ScheduledAt) AS FirstInterviewAt,
                    CAST(MIN(i.ScheduledAt) AS date) AS FirstInterviewDate,
                    DATEPART(weekday, MIN(i.ScheduledAt)) AS FirstInterviewDayOfWeek,
                    DATENAME(weekday, MIN(i.ScheduledAt)) AS FirstInterviewDayOfWeekName,
                    MAX(i.ScheduledAt) AS LastInterviewAt,
                    CAST(MAX(i.ScheduledAt) AS date) AS LastInterviewDate
                FROM dbo.JobApplications AS ja
                LEFT JOIN dbo.Interviews AS i
                    ON i.JobApplicationId = ja.Id
                GROUP BY
                    ja.Id,
                    ja.UserId,
                    ja.CompanyName,
                    ja.Position,
                    ja.AppliedAt,
                    ja.Status,
                    ja.Notes;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[dbo].[vw_JobApplicationAnalytics]', N'V') IS NOT NULL
                    DROP VIEW [dbo].[vw_JobApplicationAnalytics];
            ");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_CompanyName",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_AppliedAt",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_UserId",
                table: "JobApplications");
        }
    }
}
