using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTBay.web.Migrations
{
    /// <inheritdoc />
    public partial class AddingUsrfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Usrs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Usrs");
        }
    }
}
