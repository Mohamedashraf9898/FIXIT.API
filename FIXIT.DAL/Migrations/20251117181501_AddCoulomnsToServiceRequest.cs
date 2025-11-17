using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCoulomnsToServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "ServiceAt",
            //    table: "ServicesRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "ServicesRequests",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "ServicesRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WaitingForClientPaymentAt",
                table: "ServicesRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "ServicesRequests");

            migrationBuilder.DropColumn(
                name: "WaitingForClientPaymentAt",
                table: "ServicesRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "ServicesRequests",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceAt",
                table: "ServicesRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
