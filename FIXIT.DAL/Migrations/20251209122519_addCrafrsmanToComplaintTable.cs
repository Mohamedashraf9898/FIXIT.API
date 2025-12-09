using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addCrafrsmanToComplaintTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Complaints",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CraftsManId",
                table: "Complaints",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CraftsManId",
                table: "Complaints");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Complaints",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
