using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTBay.web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsrIdToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Usrs");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Usrs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
