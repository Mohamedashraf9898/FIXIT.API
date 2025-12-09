using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addToComplaintTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminResponse",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "Complaints");
        }
    }
}
