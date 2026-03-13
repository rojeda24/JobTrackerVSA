using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackerVSA.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInterviewNotesLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Truncate existing Interview Notes to 4000 characters to prevent SQL Server truncation errors during ALTER COLUMN
            migrationBuilder.Sql("UPDATE [Interviews] SET [Notes] = SUBSTRING([Notes], 1, 4000) WHERE LEN([Notes]) > 4000;");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Interviews",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5120,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Interviews",
                type: "nvarchar(max)",
                maxLength: 5120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);
        }
    }
}
