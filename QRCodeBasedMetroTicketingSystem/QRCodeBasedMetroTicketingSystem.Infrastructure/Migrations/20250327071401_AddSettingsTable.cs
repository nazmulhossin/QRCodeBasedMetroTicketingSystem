using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinFare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FarePerKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QrCodeValidTime = table.Column<int>(type: "int", nullable: false),
                    TripTimeLimit = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "CreatedAt", "FarePerKm", "MinFare", "QrCodeValidTime", "TripTimeLimit", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), 5.0000m, 20.0000m, 1440, 120, new DateTime(2025, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
