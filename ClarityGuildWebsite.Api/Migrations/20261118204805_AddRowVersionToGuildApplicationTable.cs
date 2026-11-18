using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToGuildApplicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildApplications");

            migrationBuilder.CreateTable(
                name: "GuildApplication",
                columns: table => new
                {
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscordId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscordName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    MainName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainRealm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AltName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AltRealm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AltRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Preparation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    About = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Tech = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    History = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Extra = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscordMessageId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiscordThreadId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastPostError = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastAttemptedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRecruitmentTeamMessagedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Retry = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostStatus = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildApplication", x => x.PublicId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplication_DiscordThreadId",
                table: "GuildApplication",
                column: "DiscordThreadId",
                unique: true,
                filter: "[DiscordThreadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplication_PublicId",
                table: "GuildApplication",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildApplication");

            migrationBuilder.CreateTable(
                name: "GuildApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    About = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    AltName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AltRealm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AltRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscordId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscordMessageId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiscordName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscordThreadId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Extra = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    History = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LastAttemptedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPostError = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastRecruitmentTeamMessagedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MainName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainRealm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostStatus = table.Column<int>(type: "int", nullable: false),
                    Preparation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Retry = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Tech = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildApplications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_DiscordThreadId",
                table: "GuildApplications",
                column: "DiscordThreadId",
                unique: true,
                filter: "[DiscordThreadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_PublicId",
                table: "GuildApplications",
                column: "PublicId",
                unique: true);
        }
    }
}
