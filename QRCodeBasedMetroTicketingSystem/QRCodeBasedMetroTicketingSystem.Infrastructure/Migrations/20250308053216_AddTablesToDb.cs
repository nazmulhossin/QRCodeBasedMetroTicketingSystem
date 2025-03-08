using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(12,8)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.StationId);
                });

            migrationBuilder.CreateTable(
                name: "StationDistances",
                columns: table => new
                {
                    DistanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Station1Id = table.Column<int>(type: "int", nullable: false),
                    Station2Id = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(12,6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationDistances", x => x.DistanceId);
                    table.ForeignKey(
                        name: "FK_StationDistances_Stations_Station1Id",
                        column: x => x.Station1Id,
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StationDistances_Stations_Station2Id",
                        column: x => x.Station2Id,
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationDistances_Station1Id",
                table: "StationDistances",
                column: "Station1Id");

            migrationBuilder.CreateIndex(
                name: "IX_StationDistances_Station2Id",
                table: "StationDistances",
                column: "Station2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Order",
                table: "Stations",
                column: "Order",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationDistances");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
