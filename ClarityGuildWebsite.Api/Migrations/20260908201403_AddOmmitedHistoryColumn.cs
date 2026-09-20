using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOmmitedHistoryColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "History",
                table: "GuildApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "History",
                table: "GuildApplications");
        }
    }
}
