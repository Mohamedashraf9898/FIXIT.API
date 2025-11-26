using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ServiceEnhance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraftsMenServices");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Amount",
            //    table: "WalletTransactions",
            //    type: "decimal(10,2)",
            //    nullable: true,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(10,2)");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "IsCancelled",
            //    table: "ServicesRequests",
            //    type: "bit",
            //    nullable: true,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit",
            //    oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "CraftsMen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CraftsMen_ServiceId",
                table: "CraftsMen",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CraftsMen_Services_ServiceId",
                table: "CraftsMen",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CraftsMen_Services_ServiceId",
                table: "CraftsMen");

            migrationBuilder.DropIndex(
                name: "IX_CraftsMen_ServiceId",
                table: "CraftsMen");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "CraftsMen");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WalletTransactions",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "ServicesRequests",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.CreateTable(
                name: "CraftsMenServices",
                columns: table => new
                {
                    CraftsManId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftsMenServices", x => new { x.CraftsManId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_CraftsMenServices_CraftsMen_CraftsManId",
                        column: x => x.CraftsManId,
                        principalTable: "CraftsMen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CraftsMenServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CraftsMenServices_ServiceId",
                table: "CraftsMenServices",
                column: "ServiceId");
        }
    }
}
