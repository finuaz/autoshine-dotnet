using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoShine.Web.Migrations
{
    /// <inheritdoc />
    public partial class ProtectHistoricalServiceReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_Services_ServiceId",
                table: "BookingItems");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_Services_ServiceId",
                table: "BookingItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_Services_ServiceId",
                table: "BookingItems");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_Services_ServiceId",
                table: "BookingItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
