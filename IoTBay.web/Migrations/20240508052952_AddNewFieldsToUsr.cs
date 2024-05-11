using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTBay.web.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToUsr : Migration
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
                name: "FullName",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "Usrs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Usrs");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "Usrs");
        }
    }
}
