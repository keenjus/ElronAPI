using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElronAPI.Migrations
{
    public partial class singularizedElronAccountstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElronAccounts_ElronPeriodTicket_ActivePeriodTicketId",
                table: "ElronAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ElronPeriodTicket_ElronAccounts_ElronAccountId",
                table: "ElronPeriodTicket");

            migrationBuilder.DropForeignKey(
                name: "FK_ElronTransaction_ElronAccounts_ElronAccountId",
                table: "ElronTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElronAccounts",
                table: "ElronAccounts");

            migrationBuilder.RenameTable(
                name: "ElronAccounts",
                newName: "ElronAccount");

            migrationBuilder.RenameIndex(
                name: "IX_ElronAccounts_ActivePeriodTicketId",
                table: "ElronAccount",
                newName: "IX_ElronAccount_ActivePeriodTicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElronAccount",
                table: "ElronAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ElronAccount_ElronPeriodTicket_ActivePeriodTicketId",
                table: "ElronAccount",
                column: "ActivePeriodTicketId",
                principalTable: "ElronPeriodTicket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElronPeriodTicket_ElronAccount_ElronAccountId",
                table: "ElronPeriodTicket",
                column: "ElronAccountId",
                principalTable: "ElronAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElronTransaction_ElronAccount_ElronAccountId",
                table: "ElronTransaction",
                column: "ElronAccountId",
                principalTable: "ElronAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElronAccount_ElronPeriodTicket_ActivePeriodTicketId",
                table: "ElronAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_ElronPeriodTicket_ElronAccount_ElronAccountId",
                table: "ElronPeriodTicket");

            migrationBuilder.DropForeignKey(
                name: "FK_ElronTransaction_ElronAccount_ElronAccountId",
                table: "ElronTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElronAccount",
                table: "ElronAccount");

            migrationBuilder.RenameTable(
                name: "ElronAccount",
                newName: "ElronAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_ElronAccount_ActivePeriodTicketId",
                table: "ElronAccounts",
                newName: "IX_ElronAccounts_ActivePeriodTicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElronAccounts",
                table: "ElronAccounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ElronAccounts_ElronPeriodTicket_ActivePeriodTicketId",
                table: "ElronAccounts",
                column: "ActivePeriodTicketId",
                principalTable: "ElronPeriodTicket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElronPeriodTicket_ElronAccounts_ElronAccountId",
                table: "ElronPeriodTicket",
                column: "ElronAccountId",
                principalTable: "ElronAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElronTransaction_ElronAccounts_ElronAccountId",
                table: "ElronTransaction",
                column: "ElronAccountId",
                principalTable: "ElronAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
