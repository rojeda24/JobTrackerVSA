using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackerVSA.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeUrlToJobApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeUrl",
                table: "JobApplications",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeUrl",
                table: "JobApplications");
        }
    }
}
