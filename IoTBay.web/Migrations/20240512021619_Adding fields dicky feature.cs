using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTBay.web.Migrations
{
    /// <inheritdoc />
    public partial class Addingfieldsdickyfeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Usrs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AccessLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_AccessLogs_Usrs_UserId",
                        column: x => x.UserId,
                        principalTable: "Usrs",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_UserId",
                table: "AccessLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "Usrs");
        }
    }
}
