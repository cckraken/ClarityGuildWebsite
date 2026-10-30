using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameLastOfficerMessaged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastOfficerMessagedUtc",
                table: "GuildApplications",
                newName: "LastRecruitmentTeamMessagedUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastRecruitmentTeamMessagedUtc",
                table: "GuildApplications",
                newName: "LastOfficerMessagedUtc");
        }
    }
}
