using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PaymentIntet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "ServicesRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "ServicesRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "ServicesRequests");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "ServicesRequests");
        }
    }
}
