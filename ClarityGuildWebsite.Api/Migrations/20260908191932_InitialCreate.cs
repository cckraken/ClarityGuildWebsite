using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscordId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarcraftLogsLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tech = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schedule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Splits = table.Column<bool>(type: "bit", nullable: true),
                    Communication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Screenshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vouch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goals = table.Column<bool>(type: "bit", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscordMessageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAttemptedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Retry = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildApplications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildApplications");
        }
    }
}
