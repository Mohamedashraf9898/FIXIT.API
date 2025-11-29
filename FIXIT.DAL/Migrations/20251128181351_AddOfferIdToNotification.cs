using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOfferIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfferId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OfferId",
                table: "Notifications",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Offers_OfferId",
                table: "Notifications",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Offers_OfferId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_OfferId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "Notifications");
        }
    }
}
