using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ElronAPI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElronAccountId = table.Column<string>(nullable: true),
                    TransactionId = table.Column<long>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronPeriodTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElronAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ActivePeriodTicketId = table.Column<long>(nullable: true),
                    Balance = table.Column<decimal>(nullable: true),
                    LastCheck = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElronAccounts_ElronPeriodTicket_ActivePeriodTicketId",
                        column: x => x.ActivePeriodTicketId,
                        principalTable: "ElronPeriodTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElronTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    ElronAccountId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sum = table.Column<decimal>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElronTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElronTransaction_ElronAccounts_ElronAccountId",
                        column: x => x.ElronAccountId,
                        principalTable: "ElronAccounts",
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
                name: "IX_ElronAccounts_ActivePeriodTicketId",
                table: "ElronAccounts",
                column: "ActivePeriodTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronPeriodTicket_ElronAccountId",
                table: "ElronPeriodTicket",
                column: "ElronAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronPeriodTicket_TransactionId",
                table: "ElronPeriodTicket",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronTransaction_ElronAccountId",
                table: "ElronTransaction",
                column: "ElronAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ElronTransaction_TicketId",
                table: "ElronTransaction",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElronPeriodTicket_ElronAccounts_ElronAccountId",
                table: "ElronPeriodTicket",
                column: "ElronAccountId",
                principalTable: "ElronAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElronPeriodTicket_ElronTransaction_TransactionId",
                table: "ElronPeriodTicket",
                column: "TransactionId",
                principalTable: "ElronTransaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElronAccounts_ElronPeriodTicket_ActivePeriodTicketId",
                table: "ElronAccounts");

            migrationBuilder.DropTable(
                name: "ElronPeriodTicket");

            migrationBuilder.DropTable(
                name: "ElronTransaction");

            migrationBuilder.DropTable(
                name: "ElronAccounts");

            migrationBuilder.DropTable(
                name: "ElronTicket");
        }
    }
}
