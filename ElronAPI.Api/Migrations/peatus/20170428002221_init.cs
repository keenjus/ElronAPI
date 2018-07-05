using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElronAPI.Api.Migrations.peatus
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agencies",
                columns: table => new
                {
                    agency_id = table.Column<long>(nullable: false),
                    agency_lang = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    agency_name = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    agency_phone = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    agency_timezone = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    agency_url = table.Column<string>(type: "varchar", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agencies", x => x.agency_id);
                });

            migrationBuilder.CreateTable(
                name: "calendar",
                columns: table => new
                {
                    service_id = table.Column<int>(nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    friday = table.Column<bool>(nullable: false),
                    monday = table.Column<bool>(nullable: false),
                    saturday = table.Column<bool>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    sunday = table.Column<bool>(nullable: false),
                    thursday = table.Column<bool>(nullable: false),
                    tuesday = table.Column<bool>(nullable: false),
                    wednesday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "stops",
                columns: table => new
                {
                    stop_id = table.Column<int>(nullable: false),
                    alias = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    lest_x = table.Column<decimal>(nullable: true),
                    lest_y = table.Column<decimal>(nullable: true),
                    stop_area = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    stop_code = table.Column<string>(type: "varchar", maxLength: 16, nullable: true),
                    stop_desc = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    stop_lat = table.Column<double>(nullable: true),
                    stop_lon = table.Column<double>(nullable: true),
                    stop_name = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    zone_id = table.Column<int>(nullable: true),
                    zone_name = table.Column<string>(type: "varchar", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stops", x => x.stop_id);
                });

            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    route_id = table.Column<string>(type: "varchar", maxLength: 32, nullable: false),
                    agency_id = table.Column<long>(nullable: true),
                    competent_authority = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    route_color = table.Column<string>(type: "varchar", maxLength: 16, nullable: true),
                    route_long_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: true),
                    route_short_name = table.Column<string>(type: "varchar", maxLength: 16, nullable: true),
                    route_type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routes", x => x.route_id);
                    table.ForeignKey(
                        name: "routes_agencies_fkey",
                        column: x => x.agency_id,
                        principalTable: "agencies",
                        principalColumn: "agency_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    trip_id = table.Column<long>(nullable: false),
                    direction_code = table.Column<string>(type: "varchar", maxLength: 32, nullable: true),
                    route_id = table.Column<string>(type: "varchar", maxLength: 32, nullable: false),
                    service_id = table.Column<int>(nullable: true),
                    shape_id = table.Column<int>(nullable: true),
                    trip_headsign = table.Column<string>(type: "varchar", maxLength: 64, nullable: true),
                    trip_long_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: true),
                    wheelchair_accessible = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.trip_id);
                    table.ForeignKey(
                        name: "trips_routes_fkey",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "route_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stop_times",
                columns: table => new
                {
                    trip_id = table.Column<long>(nullable: false),
                    stop_id = table.Column<int>(nullable: false),
                    stop_sequence = table.Column<int>(nullable: false),
                    arrival_time = table.Column<string>(type: "varchar", maxLength: 16, nullable: true),
                    departure_time = table.Column<string>(type: "varchar", maxLength: 16, nullable: true),
                    drop_off_type = table.Column<int>(nullable: true),
                    pickup_type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stop_times", x => new { x.trip_id, x.stop_id, x.stop_sequence });
                    table.ForeignKey(
                        name: "stop_times_stops_fkey",
                        column: x => x.stop_id,
                        principalTable: "stops",
                        principalColumn: "stop_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "stop_times_trips_fkey",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "trip_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_routes_agency_id",
                table: "routes",
                column: "agency_id");

            migrationBuilder.CreateIndex(
                name: "IX_stop_times_stop_id",
                table: "stop_times",
                column: "stop_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_route_id",
                table: "trips",
                column: "route_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calendar");

            migrationBuilder.DropTable(
                name: "stop_times");

            migrationBuilder.DropTable(
                name: "stops");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "routes");

            migrationBuilder.DropTable(
                name: "agencies");
        }
    }
}
