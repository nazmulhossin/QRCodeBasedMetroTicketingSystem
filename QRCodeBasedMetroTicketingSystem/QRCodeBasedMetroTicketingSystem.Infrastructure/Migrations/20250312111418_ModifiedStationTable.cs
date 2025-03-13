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
            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationName",
                table: "Stations",
                column: "StationName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stations_StationName",
                table: "Stations");
        }
    }
}
