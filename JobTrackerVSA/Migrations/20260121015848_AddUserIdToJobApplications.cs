using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackerVSA.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToJobApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "JobApplications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JobApplications");
        }
    }
}
