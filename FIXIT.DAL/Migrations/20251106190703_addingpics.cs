using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addingpics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ServicesRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceRequestImage",
                table: "ServicesRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "CraftsMen",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ServicesRequests");

            migrationBuilder.DropColumn(
                name: "ServiceRequestImage",
                table: "ServicesRequests");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "CraftsMen",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
