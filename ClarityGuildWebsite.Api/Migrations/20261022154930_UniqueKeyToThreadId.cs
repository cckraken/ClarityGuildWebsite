using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class UniqueKeyToThreadId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscordThreadId",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_DiscordThreadId",
                table: "GuildApplications",
                column: "DiscordThreadId",
                unique: true,
                filter: "[DiscordThreadId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildApplications_DiscordThreadId",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "DiscordThreadId",
                table: "GuildApplications");
        }
    }
}
