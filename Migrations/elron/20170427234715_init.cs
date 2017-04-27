using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ElronAPI.Migrations.elron
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElronAccount",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Balance = table.Column<decimal>(nullable: true),
                    BalanceThreshold = table.Column<decimal>(nullable: true),
                    LastCheck = table.Column<DateTime>(nullable: false),
                    PeriodTicketThreshold = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElronTicket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElronPeriodTicket",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ElronAccountId = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronPeriodTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElronPeriodTicket_ElronAccount_ElronAccountId",
                        column: x => x.ElronAccountId,
                        principalTable: "ElronAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElronTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    ElronAccountId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PeriodTicketId = table.Column<long>(nullable: true),
                    Sum = table.Column<decimal>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElronTransaction_ElronAccount_ElronAccountId",
                        column: x => x.ElronAccountId,
                        principalTable: "ElronAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElronTransaction_ElronPeriodTicket_PeriodTicketId",
                        column: x => x.PeriodTicketId,
                        principalTable: "ElronPeriodTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElronTransaction_ElronTicket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "ElronTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElronPeriodTicket_ElronAccountId",
                table: "ElronPeriodTicket",
                column: "ElronAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronTransaction_ElronAccountId",
                table: "ElronTransaction",
                column: "ElronAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronTransaction_PeriodTicketId",
                table: "ElronTransaction",
                column: "PeriodTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronTransaction_TicketId",
                table: "ElronTransaction",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElronTransaction");

            migrationBuilder.DropTable(
                name: "ElronPeriodTicket");

            migrationBuilder.DropTable(
                name: "ElronTicket");

            migrationBuilder.DropTable(
                name: "ElronAccount");
        }
    }
}
