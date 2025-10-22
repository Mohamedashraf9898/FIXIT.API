using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMIgrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalRequests = table.Column<int>(type: "int", nullable: false),
                    ServicesRequestId = table.Column<int>(type: "int", nullable: false),
                    CraftsManId = table.Column<int>(type: "int", nullable: false),
                    FName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InitialPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CraftsManId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "CraftsMen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Describtion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperienceOfYears = table.Column<int>(type: "int", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "money", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    ServicesRequestId = table.Column<int>(type: "int", nullable: false),
                    FName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftsMen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftsMen_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId");
                });

            migrationBuilder.CreateTable(
                name: "ServicesRequests",
                columns: table => new
                {
                    ServicesRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CraftsManId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    RequestAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesRequests", x => x.ServicesRequestId);
                    table.ForeignKey(
                        name: "FK_ServicesRequests_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServicesRequests_CraftsMen_CraftsManId",
                        column: x => x.CraftsManId,
                        principalTable: "CraftsMen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServicesRequests_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CraftsMen_ServiceId",
                table: "CraftsMen",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesRequests_ClientId",
                table: "ServicesRequests",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesRequests_CraftsManId",
                table: "ServicesRequests",
                column: "CraftsManId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesRequests_ServiceId",
                table: "ServicesRequests",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicesRequests");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "CraftsMen");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
