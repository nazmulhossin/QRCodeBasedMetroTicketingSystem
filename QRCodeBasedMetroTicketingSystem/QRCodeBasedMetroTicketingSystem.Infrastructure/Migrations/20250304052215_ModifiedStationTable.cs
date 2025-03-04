using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedStationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StationCode",
                table: "Stations");

            migrationBuilder.AlterColumn<string>(
                name: "StationName",
                table: "Stations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Stations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Stations");

            migrationBuilder.AlterColumn<string>(
                name: "StationName",
                table: "Stations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "StationCode",
                table: "Stations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
