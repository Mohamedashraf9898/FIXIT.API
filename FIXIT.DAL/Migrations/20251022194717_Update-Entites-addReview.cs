using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntitesaddReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CraftsMen_Services_ServiceId",
                table: "CraftsMen");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicesRequests_Clients_ClientId",
                table: "ServicesRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicesRequests_CraftsMen_CraftsManId",
                table: "ServicesRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicesRequests_Services_ServiceId",
                table: "ServicesRequests");

            migrationBuilder.DropIndex(
                name: "IX_CraftsMen_ServiceId",
                table: "CraftsMen");

            migrationBuilder.DropColumn(
                name: "CraftsManId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "CraftsMen");

            migrationBuilder.DropColumn(
                name: "ServicesRequestId",
                table: "CraftsMen");

            migrationBuilder.DropColumn(
                name: "CraftsManId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ServicesRequestId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestAt",
                table: "ServicesRequests",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CraftsManId",
                table: "ServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "ServicesRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialPrice",
                table: "Services",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "CraftsMen",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServicesRequestId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CraftsManId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_CraftsMen_CraftsManId",
                        column: x => x.CraftsManId,
                        principalTable: "CraftsMen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_ServicesRequests_ServicesRequestId",
                        column: x => x.ServicesRequestId,
                        principalTable: "ServicesRequests",
                        principalColumn: "ServicesRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CraftsMenServices_ServiceId",
                table: "CraftsMenServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ClientId",
                table: "Reviews",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CraftsManId",
                table: "Reviews",
                column: "CraftsManId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ServicesRequestId",
                table: "Reviews",
                column: "ServicesRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesRequests_Clients_ClientId",
                table: "ServicesRequests",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesRequests_CraftsMen_CraftsManId",
                table: "ServicesRequests",
                column: "CraftsManId",
                principalTable: "CraftsMen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesRequests_Services_ServiceId",
                table: "ServicesRequests",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicesRequests_Clients_ClientId",
                table: "ServicesRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicesRequests_CraftsMen_CraftsManId",
                table: "ServicesRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicesRequests_Services_ServiceId",
                table: "ServicesRequests");

            migrationBuilder.DropTable(
                name: "CraftsMenServices");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServicesRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ServicesRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestAt",
                table: "ServicesRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "CraftsManId",
                table: "ServicesRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "ServicesRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialPrice",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<int>(
                name: "CraftsManId",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "CraftsMen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "CraftsMen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicesRequestId",
                table: "CraftsMen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CraftsManId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServicesRequestId",
                table: "Clients",
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
                principalColumn: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesRequests_Clients_ClientId",
                table: "ServicesRequests",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesRequests_CraftsMen_CraftsManId",
                table: "ServicesRequests",
                column: "CraftsManId",
                principalTable: "CraftsMen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicesRequests_Services_ServiceId",
                table: "ServicesRequests",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId");
        }
    }
}
