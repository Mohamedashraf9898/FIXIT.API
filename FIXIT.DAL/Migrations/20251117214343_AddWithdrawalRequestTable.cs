using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdrawalRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CraftsManId",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServicesRequestId",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "WalletTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "WalletId1",
                table: "WalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WithdrawalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CraftsManId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId1",
                table: "WalletTransactions",
                column: "WalletId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId1",
                table: "WalletTransactions",
                column: "WalletId1",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId1",
                table: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "WithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_WalletId1",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "CraftsManId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "ServicesRequestId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "WalletId1",
                table: "WalletTransactions");
        }
    }
}
