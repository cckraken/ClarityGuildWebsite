using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClarityGuildWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackFrontDataRestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "Communication",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "Vouch",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "WarcraftLogsLink",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "WowClass",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "WowSpecs",
                table: "GuildApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Tech",
                table: "GuildApplications",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "History",
                table: "GuildApplications",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Goals",
                table: "GuildApplications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "GuildApplications",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AltName",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AltRealm",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AltRole",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extra",
                table: "GuildApplications",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainName",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainRealm",
                table: "GuildApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainRole",
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
                name: "About",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "AltName",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "AltRealm",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "AltRole",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "Extra",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "MainName",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "MainRealm",
                table: "GuildApplications");

            migrationBuilder.DropColumn(
                name: "MainRole",
                table: "GuildApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Tech",
                table: "GuildApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<string>(
                name: "History",
                table: "GuildApplications",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<bool>(
                name: "Goals",
                table: "GuildApplications",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "GuildApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Communication",
                table: "GuildApplications",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "GuildApplications",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Vouch",
                table: "GuildApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarcraftLogsLink",
                table: "GuildApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WowClass",
                table: "GuildApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WowSpecs",
                table: "GuildApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
