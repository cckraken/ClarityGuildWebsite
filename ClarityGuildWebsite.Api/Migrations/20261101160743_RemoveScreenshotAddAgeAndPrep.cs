using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveScreenshotAddAgeAndPrep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Screenshot",
                table: "GuildApplications");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "GuildApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Preparation",
                table: "GuildApplications",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "Preparation",
                table: "GuildApplications");

            migrationBuilder.AddColumn<string>(
                name: "Screenshot",
                table: "GuildApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
