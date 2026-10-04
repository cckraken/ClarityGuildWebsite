using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReaddCountryField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "GuildApplications");
        }
    }
}
